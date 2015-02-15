using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aura.Channel.Scripting.Ai
{
	public abstract class AiAction : DependencyObject
	{
		public string Name { get; set; } // Optional name for e.g. jumping

		public abstract void Execute();
	}
}
