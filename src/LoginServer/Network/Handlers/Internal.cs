// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Shared.Network;
using Aura.Shared.Mabi;
using Aura.Shared.Util;
using System;
using Aura.Login.Database;

namespace Aura.Login.Network.Handlers
{
	public partial class LoginServerHandlers : PacketHandlerManager<LoginClient>
	{

		/// <summary>
		/// Sent from channels to forward it to all others,
		/// message to broadcast
		/// </summary>
		/// <example>
		/// 001 [................] String : test
		/// </example>
		[PacketHandler(Op.Internal.BroadcastNotice)]
		public void Broadcast(LoginClient client, Packet packet)
		{
			LoginServer.Instance.BroadcastChannels(packet);
		}
	}
}
