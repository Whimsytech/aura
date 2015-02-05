//--- Aura Script -----------------------------------------------------------
// Go to School
//--- Description -----------------------------------------------------------
// Teaches Defense skill, started automatically after finishing
// "Rescue Resident". "about_skill" has to be selected in Ranald
// to trigger the Hook that teaches the skill. The quest is started
// and completed automatically (even if you didn't have it before).
//---------------------------------------------------------------------------

using System.Threading.Tasks;
using Aura.Channel.Scripting.Scripts;
using Aura.Shared.Mabi.Const;

public class BeginnerUladhSchool1QuestScript : QuestScript
{
	public override void Load()
	{
		SetId(1000004);
		SetName("Go to School");
		SetDescription("This is Ranald, a battle instructor at School. Battling is the most basic, fundamental aspect of an adventure. Even the ones that don't believe in fighting should at least learn to defend themselves. Please stop by the School when you can. - Ranald -");
		
		SetReceive(Receive.Automatically);
		AddPrerequisite(Completed(202002)); // Rescue Resident

		AddObjective("talk_ranald", "Talk with Ranald at Tir Chonaill School", 1, 4651, 32166, Talk("ranald"));
		
		AddReward(Exp(600));
		AddReward(Item(1612)); // Defense Guidebook
		AddReward(Skill(SkillId.Defense, SkillRank.RF));
		// TODO: Adds +2 Life according to the Wiki
		
		AddHook("_ranald", "after_intro", TalkRanald);
		AddHook("_ranald", "before_keywords", RanaldKeywords);
	}
	
	public async Task<HookResult> TalkRanald(NpcScript npc, params object[] args)
	{
		if(!npc.QuestActive(Id))
			return HookResult.Continue;
	
		return await LearnDefense(npc);
	}
	
	public async Task<HookResult> RanaldKeywords(NpcScript npc, params object[] args)
	{
		var keyword = args[0] as string;
		
		if(npc.QuestCompleted(Id) || keyword != "about_skill")
			return HookResult.Continue;
	
		return await LearnDefense(npc);
	}
	
	public async Task<HookResult> LearnDefense(NpcScript npc)
	{
		npc.Msg("It seems you are seeking a warrior's advice from me.<br/>Let's see, first, just lunging at your enemy is not everything.<br/>Defend your opponent's attack to break its flow<br/>and win a chance to strike back. It's really a critical part in a fight. <p/>That's the Defense skill.<br/>Hmm... If you haven't learned it yet, can you do me a favor?<br/>I'll let you know what it is so you can practice by yourself.");
		
		npc.Notice("Received Defense Guidebook from Ranald.");
		if(!npc.QuestActive(Id))
			npc.StartQuest(Id);
		npc.CompleteQuest(Id);
		
		npc.Msg("How to use the Defense skill is described in this book.<br/>Read it well and practice hard. That's the only efficient way you can defend yourself.");
		
		return HookResult.Break;
	}
}
