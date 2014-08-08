// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Shared.Mabi.Const;
using Aura.Shared.Network;

namespace Aura.Login.Network.Packets
{
	/// <summary>
	/// ?
	/// </summary>
	/// <remarks>
	/// No idea what this is. Answer contains a single 0 byte,
	/// possibly a list of some kind. Nothing special happens
	/// when the byte is modified.
	/// </remarks>
	[PacketHandler(Op.LoginUnk)]
	public class LoginUnk : IPacketHandler<LoginClient>
	{
		/// <example>
		/// No parameters.
		/// </example>
		public void Handle(LoginClient client, Packet packet)
		{
			LoginUnkR.Send(client, 0);
		}
	}

	/// <summary>
	/// Response to LoginUnk.
	/// </summary>
	public class LoginUnkR : IPacketSender
	{
		/// <summary>
		/// Sends LoginUnkR to client.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="unkByte"></param>
		public static void Send(LoginClient client, byte unkByte)
		{
			var packet = new Packet(Op.LoginUnkR, MabiId.Login);
			packet.PutByte(unkByte);

			client.Send(packet);
		}
	}
}
