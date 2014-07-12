// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Shared.Util;
using Aura.Shared.Util.Configuration;

namespace Aura.Msgr.Util
{
	public class MsgrConf : BaseConf
	{
		public MsgrConf()
		{
		}

		public override void Load()
		{
			this.LoadDefault();
		}
	}
}
