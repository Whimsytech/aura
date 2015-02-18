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
	public abstract class AiAction : DependencyObject
	{
		public string Name { get; set; } // Optional name for e.g. jumping

		public abstract void Execute();
	}

	[ContentProperty("Message")]
	public class Say : AiAction
	{
		public string Message { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Min")]
	public class Wait : AiAction
	{
		public int Min { get; set; }
		public int Max { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("MinDistance")]
	public class Wander : AiAction
	{
		public int MinDistance { get; set; }
		public int MaxDistance { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class MoveTo : AiAction
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
	public class Circle : AiAction
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
	public class Follow : AiAction
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
	public class KeepAway : AiAction
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
	public class Attack : AiAction
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
	public class PrepareSkill : AiAction
	{
		public SkillId Skill { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class CancelSkill : AiAction
	{
		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class CompleteSkill : AiAction
	{
		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Skill")]
	public class StartSkill : AiAction
	{
		public SkillId Skill { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Skill")]
	public class StopSkill : AiAction
	{
		public SkillId Skill { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}
}
