// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Shared.Mabi;
using Aura.Shared.Network;
using Aura.Shared.Util;

namespace Aura.Login.Network.Packets.Internal
{
	/// <summary>
	/// Sent from other servers after connecting to verify identity.
	/// </summary>
	[PacketHandler(Op.Internal.ServerIdentify)]
	public class Internal_ServerIdentify : IPacketHandler<LoginClient>
	{
		public void Handle(LoginClient client, Packet packet)
		{
			var passwordHash = packet.GetString();

			if (!Password.Check(LoginServer.Instance.Conf.Internal.Password, passwordHash))
			{
				Internal_ServerIdentifyR.Send(client, false);

				Log.Warning("Invalid internal password from '{0}'.", client.Address);
				client.Kill();
				return;
			}

			client.State = ClientState.LoggedIn;

			// TODO: No outside locking
			lock (LoginServer.Instance.ChannelClients)
				LoginServer.Instance.ChannelClients.Add(client);

			Internal_ServerIdentifyR.Send(client, true);
		}
	}

	/// <summary>
	/// Response to Internal_ServerIdentify.
	/// </summary>
	public class Internal_ServerIdentifyR : IPacketSender
	{
		/// <summary>
		/// Sends Internal.ServerIdentifyR  to channel client.
		/// </summary>
		public static void Send(LoginClient client, bool success)
		{
			var packet = new Packet(Op.Internal.ServerIdentifyR, 0);
			packet.PutByte(success);

			client.Send(packet);
		}
	}
}