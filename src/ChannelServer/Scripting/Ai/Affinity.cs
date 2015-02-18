using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Aura.Channel.Scripting.Ai
{
	[ContentProperty("RaceId")]
	public abstract class Affinity
	{
		public string RaceId { get; set; }
	}

	public class Loves : Affinity
	{
		
	}

	public class Distrusts : Affinity
	{
		
	}

	public class Hates : Affinity
	{
		
	}
}
