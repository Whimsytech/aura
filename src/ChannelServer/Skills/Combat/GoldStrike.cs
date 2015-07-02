// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Channel.Skills.Base;
using Aura.Channel.Skills.Magic;
using Aura.Channel.World.Entities;
using Aura.Data.Database;
using Aura.Mabi.Const;
using Aura.Mabi.Network;
using Aura.Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Channel.Skills.Combat
{
	[Skill(SkillId.GoldStrike)]
	public class GoldStrike : ISkillHandler, IPreparable, IReadyable, IUseable, ICompletable, ICancelable
	{
		/// <summary>
		/// Units an enemy is knocked back.
		/// </summary>
		private const int KnockBackDistance = 450;

		/// <summary>
		/// Stack amount required for instant knock back.
		/// </summary>
		private const int MinKnockBackAmount = 3;

		/// <summary>
		/// Stun for the target.
		/// </summary>
		private const int TargetStun = 2000;

		/// <summary>
		/// Stability Reduction for target.
		/// </summary>
		private const int StabilityReduction = 50;

		/// <summary>
		/// Chance for a critical hit (unofficial).
		/// </summary>
		private const int CritChance = 5;

		/// <summary>
		/// Prepares skill.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="skill"></param>
		/// <param name="packet"></param>
		/// <returns></returns>
		public bool Prepare(Creature creature, Skill skill, Packet packet)
		{
			creature.StopMove();

			var size = 1.0f + skill.RankData.Var6 * skill.Stacks;

			Send.GoldBag(creature, size);
			Send.CombatSetAimR(creature, 0, 0, 0);
			Send.SkillPrepare(creature, skill.Info.Id, skill.GetCastTime());

			return true;
		}

		/// <summary>
		/// Readies skill.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="skill"></param>
		/// <param name="packet"></param>
		/// <returns></returns>
		public bool Ready(Creature creature, Skill skill, Packet packet)
		{
			skill.Stacks = Math.Min(skill.RankData.StackMax, skill.Stacks + 1);
			Log.Debug(skill.Stacks);

			Send.SkillReady(creature, skill.Info.Id);

			return true;
		}

		/// <summary>
		/// Uses skill by throwing the gold bag.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="skill"></param>
		/// <param name="packet"></param>
		public void Use(Creature attacker, Skill skill, Packet packet)
		{
			// Parse
			var targetEntityId = packet.GetLong();
			var unkInt1 = packet.GetInt();
			var unkInt2 = packet.GetInt();

			// Get target
			var target = attacker.Region.GetCreature(targetEntityId);
			if (target == null)
			{
				Send.SkillUseSilentCancel(attacker);
				return;
			}

			target.StopMove();

			var rnd = RandomProvider.Get();
			var targetHasShield = (target.LeftHand != null && target.LeftHand.IsShield);
			var size = 1.0f + skill.RankData.Var6 * skill.Stacks;

			// Create actions
			var aAction = new AttackerAction(CombatActionType.RangeHit, attacker, skill.Info.Id, targetEntityId);
			aAction.Set(/*AttackerOptions.KnockBackHit1 | AttackerOptions.KnockBackHit2 |*/ AttackerOptions.UseEffect);
			aAction.PropId = attacker.EntityId; // ? o.o

			var tAction = new TargetAction(CombatActionType.TakeHit, target, attacker, skill.Info.Id);
			tAction.Set(TargetOptions.Result | TargetOptions.CleanHit);

			var cap = new CombatActionPack(attacker, skill.Info.Id, aAction, tAction);

			// Damage (unofficial)
			var gold = (float)Math.Pow(skill.Stacks, 2) * 100f;
			var damage = rnd.Between(skill.RankData.Var2, skill.RankData.Var3) * (gold / 100f);

			// Critical Hit
			var critChance = CritChance;
			CriticalHit.Handle(attacker, critChance, ref damage, tAction);

			// Subtract target def/prot
			SkillHelper.HandleDefenseProtection(target, ref damage);

			// Defense
			Defense.Handle(aAction, tAction, ref damage);

			// Mana Shield
			ManaShield.Handle(target, ref damage, tAction);

			// Take it
			if (damage > 0)
				target.TakeDamage(tAction.Damage = damage, attacker);

			// Aggro
			target.Aggro(attacker);

			// Evaluate caused damage
			if (!target.IsDead)
			{
				if (!targetHasShield)
				{
					if (skill.Stacks >= MinKnockBackAmount)
						target.Stability -= Creature.MinStability;
					else
						target.Stability -= StabilityReduction;
				}

				if (target.IsUnstable && target.Is(RaceStands.KnockBackable))
					tAction.Set(TargetOptions.KnockDown);
			}
			else
			{
				tAction.Set(TargetOptions.FinishingKnockDown);
			}

			// React to knock back
			if (tAction.IsKnockBack)
			{
				attacker.Shove(target, KnockBackDistance);
				aAction.Set(AttackerOptions.KnockBackHit1 | AttackerOptions.KnockBackHit2);
			}

			// Set stun time
			if (tAction.Type != CombatActionType.Defended || tAction.IsKnockBack)
				tAction.Stun = TargetStun;

			// Effects
			var delay = 571;

			Send.Effect(attacker, Effect.GoldStrike, (byte)0, targetEntityId, delay, size);

			Send.SkillUse(attacker, skill.Info.Id, targetEntityId, delay);
			skill.Stacks = 0;

			System.Threading.Thread.Sleep(delay);

			Send.Effect(attacker, Effect.GoldStrike, (byte)1, targetEntityId, (byte)0);

			// Handling
			cap.Handle();
		}

		/// <summary>
		/// Completes skill.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="skill"></param>
		/// <param name="packet"></param>
		public void Complete(Creature creature, Skill skill, Packet packet)
		{
			var targetEntityId = packet.GetLong();
			var unkInt = packet.GetInt();

			this.Cancel(creature, skill);

			Send.SkillComplete(creature, skill.Info.Id, targetEntityId, unkInt);
		}

		/// <summary>
		/// Cancels skill by removing gold bag.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="skill"></param>
		public void Cancel(Creature creature, Skill skill)
		{
			Send.RemoveGoldBag(creature, 0);
		}
	}
}
