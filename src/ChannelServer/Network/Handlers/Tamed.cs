// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Channel.Util;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.Channel.World;
using Aura.Mabi.Const;
using Aura.Channel.World.Inventory;
using Aura.Mabi.Network;

namespace Aura.Channel.Network.Handlers
{
	public partial class ChannelServerHandlers : PacketHandlerManager<ChannelClient>
	{
		/// <summary>
		/// Sent by tamed creatures to attack.
		/// </summary>
		/// <example>
		/// 001 [0010F000000000BE] Long   : 4767482418036926
		/// </example>
		[PacketHandler(Op.TamedCombatAttack)]
		public void TamedCombatAttack(ChannelClient client, Packet packet)
		{
			this.CombatAttack(client, packet);
		}

		/// <summary>
		/// Sent when tamed creature is too far away from its master.
		/// </summary>
		/// <example>
		/// 001 [..............01] Byte   : 1
		/// 002 [........00000001] Int    : 1
		/// 003 [........00001C16] Int    : 7190
		/// 004 [........00007435] Int    : 29749
		/// </example>
		[PacketHandler(Op.TamedWarp)]
		public void TamedWarp(ChannelClient client, Packet packet)
		{
			var unkByte = packet.GetByte();
			var regionId = packet.GetInt();
			var x = packet.GetInt();
			var y = packet.GetInt();

			var creature = client.GetCreatureSafe(packet.Id);
			if (!creature.Has(CreatureStates.Npc))
			{
				Send.TamedWarpR(creature, false);
				return;
			}

			creature.Jump(x, y);

			Send.TamedWarpR(creature, true);
		}

		/// <summary>
		/// Sent when tamed creature is too far away from its master.
		/// </summary>
		/// <example>
		/// 001 [0010F00000000053] Long   : 4767482418036819
		/// </example>
		[PacketHandler(Op.TamedCancel)]
		public void TamedCancel(ChannelClient client, Packet packet)
		{
			var entityId = packet.GetLong();

			var creature = client.GetCreatureSafe(packet.Id);
			var npc = client.GetCreatureSafe(entityId);

			if (!npc.Has(CreatureStates.Npc))
			{
				Send.TamedCancelR(creature, false);
				return;
			}

			npc.Disappear();

			Send.TamedUnsetMaster(creature, npc);
			Send.Notice(creature, Localization.Get("{0} has run away."), npc.RaceData.Name);
			Send.TamedControl(null, npc);
			Send.Disappear(npc);

			Send.TamedCancelR(creature, true);
		}

		/// <summary>
		/// Sent after giving control over a creature to the client.
		/// </summary>
		/// <remarks>
		/// Server doesn't seem to send a response for this, is it just to tell
		/// the server that the client now controls the creature?
		/// </remarks>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.TamedControlAcknowledged)]
		public void TamedControlAcknowledged(ChannelClient client, Packet packet)
		{
		}
	}
}
