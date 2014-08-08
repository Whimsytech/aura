// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Shared.Network;

namespace Aura.Login.Network.Packets.Internal
{
	public class Internal_BroadcastNotice : IPacketHandler<LoginClient>
	{
		/// <summary>
		/// Sent from channels to forward it to all others,
		/// message to broadcast
		/// </summary>
		/// <example>
		/// 001 [................] String : test
		/// </example>
		[PacketHandler(Op.Internal.BroadcastNotice)]
		public void Handle(LoginClient client, Packet packet)
		{
			LoginServer.Instance.BroadcastChannels(packet);
		}
	}
}
