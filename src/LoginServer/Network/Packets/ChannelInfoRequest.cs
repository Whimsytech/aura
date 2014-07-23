// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Login.Network;
using Aura.Shared.Mabi.Const;
using Aura.Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aura.Login.Network.Packets
{
	/// <summary>
	/// Request for connection information for the channel.
	/// </summary>
	/// <remarks>
	/// Sent after disconnect info, which makes the client stuck if the
	/// connection to the channel fails.
	/// </remarks>
	[PacketHandler(Op.ChannelInfoRequest)]
	public class ChannelInfoRequest : PacketHandler<LoginClient>
	{
		/// <example>
		/// Normal
		/// 0001 [................] String : Aura
		/// 0002 [................] String : Ch1
		/// 0003 [..............00] Byte   : 0
		/// 0004 [........00000001] Int    : 1
		/// 0005 [0010000000000002] Long   : 4503599627370498
		/// 
		/// Rebirth
		/// 0001 [................] String : Aura
		/// 0002 [................] String : Ch1
		/// 0003 [..............01] Byte   : 1
		/// 0004 [0010000000000002] Long   : 4503599627370498
		/// 0005 [0000000000000000] Long   : 0
		/// </example>
		public override void Handle(LoginClient client, Packet packet)
		{
			var serverName = packet.GetString();
			var channelName = packet.GetString();
			var rebirth = packet.GetBool();
			if (!rebirth)
				packet.GetInt(); // unk
			var characterId = packet.GetLong();

			// Check channel and character
			var channelInfo = LoginServer.Instance.ServerList.GetChannel(serverName, channelName);
			var character = client.Account.GetCharacter(characterId);
			if (character == null)
				character = client.Account.GetPet(characterId);

			if (channelInfo == null || character == null)
			{
				ChannelInfoRequestR.SendFail(client);
				return;
			}

			// Success
			ChannelInfoRequestR.Send(client, channelInfo, characterId);
		}
	}

	/// <summary>
	/// Response to ChannelInfoRequest.
	/// </summary>
	public class ChannelInfoRequestR : PacketHandler<LoginClient>
	{
		/// <summary>
		/// Sends negative ChannelInfoRequestR to client.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="info"></param>
		public static void SendFail(LoginClient client)
		{
			Send(client, null, 0);
		}

		/// <summary>
		/// Sends ChannelInfoRequestR to client.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="info">Negative response if null.</param>
		public static void Send(LoginClient client, ChannelInfo info, long characterId)
		{
			var packet = new Packet(Op.ChannelInfoRequestR, MabiId.Channel);
			packet.PutByte(info != null);

			if (info != null)
			{
				packet.PutString(info.ServerName);
				packet.PutString(info.Name);
				packet.PutShort(6); // Channel "Id"? (seems to be equal to channel nr)
				packet.PutString(info.Host);
				packet.PutString(info.Host);
				packet.PutShort((short)info.Port);
				packet.PutShort((short)(info.Port + 2));
				packet.PutInt(1);
				packet.PutLong(characterId);
			}

			client.Send(packet);
		}
	}
}
