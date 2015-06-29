// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending.Helpers;
using Aura.Channel.World.Entities;
using Aura.Mabi.Const;
using Aura.Mabi.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Channel.Network.Sending
{
	public static partial class Send
	{
		/// <summary>
		/// Sends TamedWarpR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="success"></param>
		public static void TamedWarpR(Creature creature, bool success)
		{
			var packet = new Packet(Op.TamedWarpR, creature.EntityId);
			packet.PutByte(success);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends TamedCancelR to creature's client.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="success"></param>
		public static void TamedCancelR(Creature creature, bool success)
		{
			var packet = new Packet(Op.TamedCancelR, creature.EntityId);
			packet.PutByte(success);

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends PrivateCreatureInfo to controller's client.
		/// </summary>
		/// <remarks>
		/// Required for the client to be able to control a new creature.
		/// </remarks>
		/// <param name="controller"></param>
		/// <param name="creature"></param>
		public static void PrivateCreatureInfo(Creature controller, Creature creature)
		{
			var packet = new Packet(Op.PrivateCreatureInfo, MabiId.Channel);
			packet.PutLong(controller.EntityId);
			packet.AddCreatureInfo(creature, CreaturePacketType.Private);

			controller.Client.Send(packet);
		}

		/// <summary>
		/// Sends TamedControl to creature's client.
		/// </summary>
		/// <remarks>
		/// This gives client control over a creature.
		/// </remarks>
		/// <param name="controller">Parameters are 0 if null.</param>
		/// <param name="creature"></param>
		public static void TamedControl(Creature controller, Creature creature)
		{
			var packet = new Packet(Op.TamedControl, creature.EntityId);
			if (controller != null)
			{
				packet.PutLong(controller.EntityId);
				packet.PutByte(1);
				packet.PutByte(1); // No AI control if 0, just following.
				packet.PutByte(1);
				packet.PutInt(600000);
				packet.PutByte(1);
			}
			else
			{
				packet.PutLong(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutByte(1);
				packet.PutInt(0);
				packet.PutByte(0);
			}

			creature.Client.Send(packet);
		}

		/// <summary>
		/// Sends TamedSetMaster to creature's client.
		/// </summary>
		/// <remarks>
		/// Sets creature's master? Makes creature actually follow controller.
		/// </remarks>
		/// <param name="controller"></param>
		/// <param name="creature"></param>
		public static void TamedSetMaster(Creature controller, Creature creature)
		{
			var packet = new Packet(Op.TamedSetMaster, controller.EntityId);
			packet.PutLong(creature.EntityId);
			packet.PutInt(1);

			controller.Client.Send(packet);
		}

		/// <summary>
		/// Sends TamedUnsetMaster to creature's client.
		/// </summary>
		/// <remarks>
		/// Unsets creature's master?
		/// </remarks>
		/// <param name="controller"></param>
		/// <param name="creature"></param>
		public static void TamedUnsetMaster(Creature controller, Creature creature)
		{
			var packet = new Packet(Op.TamedSetMaster, controller.EntityId);
			packet.PutLong(creature.EntityId);

			controller.Client.Send(packet);
		}
	}
}
