// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using Aura.Channel.Network.Sending;
using Aura.Channel.World.Entities;
using Aura.Mabi;
using Aura.Shared.Util;
using Aura.Channel.World.Dungeons.Puzzles;

namespace Aura.Channel.World.Dungeons.Props
{
	public class Chest : DungeonProp
	{
		public List<Item> _items { get; protected set; }

		public Chest(int propId, string name)
			: base(propId, 0, 0, 0, 0, 1, 0, "", "", "")
		{
			_items = new List<Item>();

			this.InternalName = name;
			this.Behavior = this.DefaultBehavior;
		}

		public Chest(Puzzle puzzle, string name)
			: base(puzzle.Dungeon.Data.ChestId, 0, 0, 0, 0, 1, 0, "", "", "")
		{
			_items = new List<Item>();

			this.InternalName = name;
			this.Behavior = this.DefaultBehavior;
		}

		private void DefaultBehavior(Creature creature, Prop prop)
		{
			if (this.State == "open")
				return;

			this.SetState("open");
			this.DropItems();
		}

		public void DropItems()
		{
			lock (_items)
			{
				foreach (var item in _items)
					item.Drop(this.Region, this.GetPosition());
				_items.Clear();
			}
		}

		/// <summary>
		/// Adds item to chest.
		/// </summary>
		/// <param name="item"></param>
		public void Add(Item item)
		{
			lock (_items)
				_items.Add(item);
		}

		/// <summary>
		/// Adds gold stacks based on amount to chest.
		/// </summary>
		/// <param name="amount"></param>
		public void AddGold(int amount)
		{
			while (amount > 0)
			{
				var n = Math.Min(1000, amount);
				amount -= n;

				var gold = Item.CreateGold(n);
				this.Add(gold);
			}
		}
	}

	//public class TreasureChest_temp : Chest
	//{
	//	private TreasureChest_temp(int id, int regionId, int x, int y, float direction, float scale = 1f, float altitude = 0,
	//		string state = "", string name = "", string title = "")
	//		: base(id, regionId, x, y, direction, scale, altitude, state, name, title)
	//	{
	//		this.Behavior = TreasureChestBehavior + this.Behavior;
	//	}

	//	private void TreasureChestBehavior(Creature creature, Prop prop)
	//	{
	//		// Make sure the chest was still closed when it was clicked.
	//		// No security violation because it could be caused by lag.
	//		if (this.State == "open")
	//			return;

	//		if (!creature.Inventory.Has(70028)) // Treasure Chest Key
	//		{
	//			// Unofficial
	//			Send.Notice(creature, Localization.Get("You don't have a key."));
	//			return;
	//		}
	//		creature.Inventory.Remove(70028);
	//	}
	//}
}
