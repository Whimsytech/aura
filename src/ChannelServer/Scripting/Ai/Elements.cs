// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Aura.Shared.Mabi.Const;

namespace Aura.Channel.Scripting.Ai
{
	public abstract class Element : DependencyObject
	{
		public string Name { get; set; } // Optional name for e.g. jumping

		public abstract void Execute();
	}

	[ContentProperty("Elements")]
	public class Sequence : Element
	{
		public ElementCollection Elements { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	// Actions -----------------------------

	[ContentProperty("Message")]
	public class Say : Element
	{
		public string Message { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Min")]
	public class Wait : Element
	{
		public int Min { get; set; }
		public int Max { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("MinDistance")]
	public class Wander : Element
	{
		public int MinDistance { get; set; }
		public int MaxDistance { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class MoveTo : Element
	{
		public int X { get; set; }
		public int Y { get; set; }
		public bool Walk { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Radius")]
	public class Circle : Element
	{
		public int Radius { get; set; }
		public int TimeMin { get; set; }
		public int TimeMax { get; set; }
		public bool Walk { get; set; }
		public bool? Clockwise { get; set; }

		public Circle()
		{
			TimeMin = 1000;
			TimeMax = 5000;
			Clockwise = null;
			Walk = true;
		}

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("MaxDistance")]
	public class Follow : Element
	{
		public int MaxDistance { get; set; }
		public bool Walk { get; set; }
		public int Timeout { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("MinDistance")]
	public class KeepAway : Element
	{
		public int MinDistance { get; set; }
		public bool Walk { get; set; }
		public int Timeout { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Count")]
	public class Attack : Element
	{
		public int Count { get; set; }
		public int Timeout { get; set; }

		public Attack()
		{
			Count = 0;
			Timeout = 30000;
		}

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Skill")]
	public class PrepareSkill : Element
	{
		public SkillId Skill { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class CancelSkill : Element
	{
		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class CompleteSkill : Element
	{
		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Skill")]
	public class StartSkill : Element
	{
		public SkillId Skill { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Skill")]
	public class StopSkill : Element
	{
		public SkillId Skill { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}
}
