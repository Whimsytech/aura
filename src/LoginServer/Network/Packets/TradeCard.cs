// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Data;
using Aura.Login.Database;
using Aura.Login.Network;
using Aura.Shared.Mabi.Const;
using Aura.Shared.Network;
using Aura.Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aura.Login.Network.Packets
{
	/// <summary>
	/// Sent when player chooses to trade a character card for
	/// the items and pons.
	/// </summary>
	/// <remarks>
	/// New in NA188.
	/// </remarks>
	[PacketHandler(Op.TradeCard)]
	public class TradeCard : PacketHandler<LoginClient>
	{
		/// <example>
		/// 001 [................] String : Zerono
		/// 002 [................] String : Aura
		/// 003 [........00000001] Int    : 1
		/// 004 [0000000000000005] Long   : 5
		/// </example>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		public override void Handle(LoginClient client, Packet packet)
		{
			var name = packet.GetString();
			var server = packet.GetString();
			var cardType = packet.GetInt();
			var cardId = packet.GetLong();

			// Check card
			var card = client.Account.GetCharacterCard(cardId);
			if (card == null)
			{
				Log.Warning("TradeCard: User '{0}' tried to trade a card he doesn't have.", client.Account.Name);
				goto L_Fail;
			}

			// Check target
			var character = client.Account.Characters.FirstOrDefault(a => a.Name == name);
			if (character == null)
			{
				Log.Warning("TradeCard: User '{0}' tried to trade a card with an invalid character.", client.Account.Name);
				goto L_Fail;
			}

			// Check card data
			var cardData = AuraData.CharCardDb.Find(card.Type);
			if (cardData == null)
			{
				Log.Warning("TradeCard: User '{0}' tried to trade a card that's not in the database.", client.Account.Name);
				goto L_Fail;
			}

			// Check trading goods
			if (cardData.TradeItem == 0 && cardData.TradePoints == 0)
				goto L_Fail;

			// Add goods
			LoginDb.Instance.TradeCard(character, cardData);

			// Success
			TradeCardR.Send(client, cardId);

		L_Fail:
			TradeCardR.Send(client, 0);
		}
	}

	/// <summary>
	/// Response to TradeCard.
	/// </summary>
	public class TradeCardR : PacketHandler<LoginClient>
	{
		/// <summary>
		/// Sends negative TradeCardR to client (temp).
		/// </summary>
		/// <param name="client"></param>
		/// <param name="cardId">Negative response if 0.</param>
		public static void Send(LoginClient client, long cardId)
		{
			var packet = new Packet(Op.TradeCardR, MabiId.Login);
			packet.PutByte(cardId != 0);
			if (cardId != 0)
				packet.PutLong(cardId);

			client.Send(packet);
		}
	}
}
