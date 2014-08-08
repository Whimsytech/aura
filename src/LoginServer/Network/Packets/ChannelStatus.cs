// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Login.Network.Packets.Helpers;
using Aura.Shared.Mabi.Const;
using Aura.Shared.Network;

namespace Aura.Login.Network.Packets
{
	/// <summary>
	/// Contains information about all servers and channels.
	/// </summary>
	public class ChannelStatus : IPacketSender
	{
		/// <summary>
		/// Sends server/channel status update to all connected clients,
		/// incl channels.
		/// </summary>
		public static void Send()
		{
			var packet = new Packet(Op.ChannelStatus, MabiId.Login);
			packet.PutByte((byte)LoginServer.Instance.ServerList.List.Count);
			foreach (var server in LoginServer.Instance.ServerList.List)
				packet.Add(server);

			LoginServer.Instance.Broadcast(packet);
		}
	}
}
