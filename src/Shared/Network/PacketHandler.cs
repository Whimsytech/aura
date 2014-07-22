// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aura.Shared.Network
{
	public abstract class PacketHandler<TClient> where TClient : BaseClient
	{
		public virtual void Handle(TClient client, Packet Packet) { }
	}
}
