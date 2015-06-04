// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Channel.World.Entities;
using Aura.Data;
using Aura.Data.Database;
using Aura.Mabi;
using Aura.Mabi.Const;
using Aura.Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aura.Channel.World
{
	/// <summary>
	/// Manages spawning and information about field bosses.
	/// </summary>
	public class FieldBossManager
	{
		private List<FieldBoss> _bosses;

		/// <summary>
		/// Creates new field boss manager.
		/// </summary>
		public FieldBossManager()
		{
			_bosses = new List<FieldBoss>();
		}

		/// <summary>
		/// Loads bosses from data.
		/// </summary>
		public void Init()
		{
			foreach (var data in AuraData.FieldBossDb.Entries.Values)
				_bosses.Add(new FieldBoss(data));
		}
	}

	public class FieldBoss
	{
		private object _sync = new object();

		private List<NPC> _spawnedBosses, _spawnedMinions;
		private int _bossKillCount;
		private DateTime _despawn;

		public FieldBossGroupData Data { get; private set; }
		public DateTime NextSpawn { get; private set; }

		public FieldBoss(FieldBossGroupData data)
		{
			var rnd = RandomProvider.Get();

			this.Data = data;
			this.NextSpawn = DateTime.Now.AddSeconds(rnd.Next(this.Data.TimeMin, this.Data.TimeMax + 1));

			_spawnedBosses = new List<NPC>();
			_spawnedMinions = new List<NPC>();

			//ChannelServer.Instance.Events.MabiTick += this.OnMabiTick;
			ChannelServer.Instance.Events.SecondsTimeTick += this.OnMabiTick;
		}

		private void OnMabiTick(ErinnTime time)
		{
			lock (_sync)
			{
				if (_spawnedBosses.Count != 0)
				{
					if (time.DateTime > _despawn)
						this.Restart();
					return;
				}
			}

			if (time.DateTime < this.NextSpawn)
				return;

			var rnd = RandomProvider.Get();

			var setData = this.Data.Sets[rnd.Next(this.Data.Sets.Count)];
			var locationData = this.Data.Locations[rnd.Next(this.Data.Locations.Count)];

			Location location;
			try
			{
				location = new Location(locationData.Location);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "FieldBoss.OnMabiTick: Error while resolving location '{0}'.", locationData.Location);
				return;
			}

			var region = ChannelServer.Instance.World.GetRegion(location.RegionId);
			if (region == null)
			{
				Log.Error("FieldBoss.OnMabiTick: Region '{0}' not found.", location.RegionId);
				return;
			}

			foreach (var enemyData in setData.Bosses)
			{
				var npc = ChannelServer.Instance.World.SpawnManager.Spawn(enemyData.RaceId, location.RegionId, location.X + enemyData.X, location.Y + enemyData.Y, true, true);
				npc.Death += this.OnBossDeath;
				_spawnedBosses.Add(npc);
			}

			foreach (var enemyData in setData.Minions)
			{
				var npc = ChannelServer.Instance.World.SpawnManager.Spawn(enemyData.RaceId, location.RegionId, location.X + enemyData.X, location.Y + enemyData.Y, true, true);
				_spawnedMinions.Add(npc);
			}

			_despawn = DateTime.Now.AddSeconds(this.Data.LifeTime);

			Send.Notice(region, NoticeType.Top, 20000, Localization.Get("{0} has appeared in {1}."), setData.Name, locationData.Name);
		}

		private void Restart()
		{
			var rnd = RandomProvider.Get();

			foreach (var boss in _spawnedBosses)
			{
				// TODO: Despawn method?

				var pos = boss.GetPosition();
				Send.SpawnEffect(SpawnEffect.MonsterDespawn, boss.RegionId, pos.X, pos.Y, boss, boss);

				boss.Region.RemoveCreature(boss);
			}

			foreach (var minion in _spawnedMinions)
			{
				var pos = minion.GetPosition();
				Send.SpawnEffect(SpawnEffect.MonsterDespawn, minion.RegionId, pos.X, pos.Y, minion, minion);

				minion.Region.RemoveCreature(minion);
			}

			_spawnedBosses.Clear();
			_spawnedMinions.Clear();
			_bossKillCount = 0;
			_despawn = DateTime.MaxValue;

			this.NextSpawn = DateTime.Now.AddSeconds(rnd.Next(this.Data.TimeMin, this.Data.TimeMax + 1));
		}

		private void OnBossDeath(Creature creature, Creature killer)
		{
			lock (_sync)
			{
				if (_spawnedBosses.Count == 0)
					return;

				_bossKillCount++;

				Send.Notice(creature.Region, NoticeType.Top, 20000, Localization.Get("{0} has been killed by {1}."), creature.Name, killer.Name);

				if (_bossKillCount < _spawnedBosses.Count)
					return;

				this.Restart();
			}
		}
	}
}
