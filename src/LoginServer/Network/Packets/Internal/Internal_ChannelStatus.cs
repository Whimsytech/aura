// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Login.Database;
using Aura.Shared.Network;
using Aura.Shared.Util;
using System;

namespace Aura.Login.Network.Packets.Internal
{
	/// <summary>
	/// Regularly sent by channels, to update some numbers.
	/// </summary>
	[PacketHandler(Op.Internal.ChannelStatus)]
	public class Internal_ChannelStatus : IPacketHandler<LoginClient>
	{
		/// <example>
		/// ...
		/// </example>
		public void Handle(LoginClient client, Packet packet)
		{
			if (client.State != ClientState.LoggedIn)
				return;

			var serverName = packet.GetString();
			var channelName = packet.GetString();
			var host = packet.GetString();
			var port = packet.GetInt();
			var users = packet.GetInt();
			var maxUsers = packet.GetInt();
			var state = (ChannelState)packet.GetInt();

			var server = LoginServer.Instance.ServerList.Add(serverName);

			ChannelInfo channel;
			server.Channels.TryGetValue(channelName, out channel);
			if (channel == null)
			{
				channel = new ChannelInfo(channelName, serverName, host, port);
				server.Channels.Add(channelName, channel);

				Log.Info("New channel registered: {0}", channel.FullName);
			}

			// A way to identify the channel of this client
			if (client.Account == null)
			{
				client.Account = new Account();
				client.Account.Name = channel.FullName;
			}

			if (channel.State != state)
			{
				Log.Status("Channel '{0}' is now in '{1}' mode.", channel.FullName, state);
			}

			channel.Host = host;
			channel.Port = port;
			channel.Users = users;
			channel.MaxUsers = maxUsers;
			channel.LastUpdate = DateTime.Now;
			channel.State = state;

			ChannelStatus.Send();
		}
	}
}
