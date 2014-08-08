// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Shared.Network;
using System;

namespace Aura.Login.Network.Packets.Helpers
{
	public static class ServerInfoHelper
	{
		/// <summary>
		/// Adds server and channel information to packet.
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="server"></param>
		public static void Add(this Packet packet, ServerInfo server)
		{
			packet.PutString(server.Name);
			packet.PutShort(0); // Server type?
			packet.PutShort(0);
			packet.PutByte(1);

			// Channels
			// ----------------------------------------------------------
			packet.PutInt(server.Channels.Count);
			foreach (var channel in server.Channels.Values)
			{
				var state = channel.State;
				if ((DateTime.Now - channel.LastUpdate).TotalSeconds > 90)
					state = ChannelState.Maintenance;

				packet.PutString(channel.Name);
				packet.PutInt((int)state);
				packet.PutInt((int)channel.Events);
				packet.PutInt(0); // 1 for Housing? Hidden?
				packet.PutShort(channel.Stress);
			}
		}

	}
}
