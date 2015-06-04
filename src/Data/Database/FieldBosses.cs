// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Data.Database
{
	public class FieldBossGroupData
	{
		public string Name { get; set; }
		public int TimeMin { get; set; }
		public int TimeMax { get; set; }
		public int LifeTime { get; set; }

		public List<FieldBossSetData> Sets { get; set; }
		public List<FieldLocationData> Locations { get; set; }

		public FieldBossGroupData()
		{
			this.Sets = new List<FieldBossSetData>();
			this.Locations = new List<FieldLocationData>();
		}
	}

	public class FieldBossSetData
	{
		public string Name { get; set; }

		public List<FieldBossEnemyData> Bosses { get; set; }
		public List<FieldBossEnemyData> Minions { get; set; }

		public FieldBossSetData()
		{
			this.Bosses = new List<FieldBossEnemyData>();
			this.Minions = new List<FieldBossEnemyData>();
		}
	}

	public class FieldBossEnemyData
	{
		public int RaceId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		// Drops
	}

	public class FieldLocationData
	{
		public string Name { get; set; }
		public string Location { get; set; }
	}

	public class FieldBossDb : DatabaseJsonIndexed<string, FieldBossGroupData>
	{
		protected override void ReadEntry(JObject entry)
		{
			entry.AssertNotMissing("name", "timeMin", "timeMax", "lifeTime", "sets", "locations");

			var groupData = new FieldBossGroupData();
			groupData.Name = entry.ReadString("name");
			groupData.TimeMin = Math.Max(0, entry.ReadInt("timeMin"));
			groupData.TimeMax = Math.Max(0, entry.ReadInt("timeMax"));
			groupData.LifeTime = entry.ReadInt("lifeTime");

			if (groupData.TimeMax < groupData.TimeMin)
				groupData.TimeMax = groupData.TimeMin;

			// Locations
			foreach (JObject locationEntry in entry["locations"])
			{
				locationEntry.AssertNotMissing("name", "location");

				var locationData = new FieldLocationData();
				locationData.Name = locationEntry.ReadString("name");
				locationData.Location = locationEntry.ReadString("location");

				groupData.Locations.Add(locationData);
			}

			// Sets
			foreach (JObject setEntry in entry["sets"])
			{
				setEntry.AssertNotMissing("name", "bosses");

				var setData = new FieldBossSetData();
				setData.Name = setEntry.ReadString("name");

				// Bosses
				foreach (JObject bossEntry in setEntry["bosses"])
				{
					bossEntry.AssertNotMissing("raceId", "x", "y");

					var bossData = new FieldBossEnemyData();
					bossData.RaceId = bossEntry.ReadInt("raceId");
					bossData.X = bossEntry.ReadInt("x");
					bossData.Y = bossEntry.ReadInt("y");

					setData.Bosses.Add(bossData);
				}

				// Minions
				if (setEntry.ContainsKey("minions"))
				{
					foreach (JObject minionEntry in setEntry["minions"])
					{
						minionEntry.AssertNotMissing("raceId", "x", "y");

						var minionData = new FieldBossEnemyData();
						minionData.RaceId = minionEntry.ReadInt("raceId");
						minionData.X = minionEntry.ReadInt("x");
						minionData.Y = minionEntry.ReadInt("y");

						setData.Minions.Add(minionData);
					}
				}

				groupData.Sets.Add(setData);
			}

			this.Entries[groupData.Name] = groupData;
		}
	}
}
