// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

namespace Aura.Shared.Network
{
	public interface IPacketHandler<TClient> where TClient : BaseClient
	{
		void Handle(TClient client, Packet packet);
	}

	public interface IPacketSender
	{
	}
}
