// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Aura.Channel.Scripting.Ai
{
	[ContentProperty("Actions")]
	public class Ai
	{
		public string Name { get; set; }

		public List<AiAction> Actions { get; set; }

		public Ai()
		{
			Actions = new List<AiAction>();
		}
	}
}
