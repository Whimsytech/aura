// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Aura.Channel.World.Entities;
using Aura.Channel.World.Inventory;
using Aura.Shared.Mabi.Const;
using CSScriptLibrary;

namespace Aura.Channel.Scripting.Ai
{
	public abstract class Element
	{
		public string Name { get; set; } // Optional name for e.g. jumping

		public abstract void Execute();
	}

	[ContentProperty("Elements")]
	public class Sequence : Element
	{
		public ElementCollection Elements { get; set; }

		public Sequence()
		{
			Elements = new ElementCollection();
		}

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class Handler : Sequence
	{
		public AiEvent Event { get; set; }
	}

	[ContentProperty("Cases")]
	public class Random : Element
	{
		public CaseCollection Cases { get; set; }

		public Random()
		{
			Cases = new CaseCollection();
		}

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Elements")]
	public class Case
	{
		public double Rate { get; set; }

		public ElementCollection Elements { get; set; }

		public Case()
		{
			Elements = new ElementCollection();
		}

		public void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Then")]
	public class If : Element
	{
		private string _cond;

		public string Condition
		{
			get
			{
				return _cond;
			}
			set
			{
				dynamic script = CSScript.Evaluator.LoadCode(string.Format(
@"using System;
using Aura.Channel.World.Entities;

public class Script
{{
	bool Test(Creature c) {{ return {0}; }}
}}", value));

				ConditionFunc = script.Test;

				_cond = value;
			}
		}

		public Func<Creature, bool> ConditionFunc { get; private set; }

		public ElementCollection Then { get; set; }
		public ElementCollection Else { get; set; }

		public If()
		{
			Then = new ElementCollection();
			Else = new ElementCollection();
		}

		public override void Execute()
		{
			/*
			var path = Else;

			if (ConditionFunc(creature))
				path = Then;

			foreach (var e in path)
				e.Execute();
			*/

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

	public class Wait : Element
	{
		public int Min { get; set; }
		public int Max { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

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

	public class Aggro : Element
	{
		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

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

	public class LevelUp : Element
	{
		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class FullHeal : Element
	{
		public bool Health { get; set; }
		public bool Stamina { get; set; }
		public bool Mana { get; set; }
		public bool Wounds { get; set; }
		public bool Hunger { get; set; }

		public FullHeal()
		{
			Health = Stamina = Mana = Wounds = Hunger = true;
		}

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	// Placeholder until we figure out exactly what summoned mobs need
	// Race, Title, Stats, AI, Count, Color, Spawn Radius, other?
	public class Summon : Element
	{
		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	[ContentProperty("Parameters")]
	public class Effect : Element
	{
		public int EffectId { get; set; }

		public ObjectCollection Parameters { get; set; }

		public Effect()
		{
			Parameters = new ObjectCollection();
		}

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class Motion : Element
	{
		public int Category { get; set; }
		public int Type { get; set; }
		public bool Loop { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class CancelMotion : Element
	{
		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class SwitchWeapon : Element
	{
		public WeaponSet? To { get; set; }

		public SwitchWeapon()
		{
			To = null;
		}

		public override void Execute()
		{
			// If null then toggle else set
			throw new NotImplementedException();
		}
	}

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

	public class StartSkill : Element
	{
		public SkillId Skill { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class StopSkill : Element
	{
		public SkillId Skill { get; set; }

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}

	public enum AiEvent
	{
		Hit,
		DefenseHit,
		KnockDown,
	}
}
