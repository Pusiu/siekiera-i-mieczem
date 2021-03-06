using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DialogueSetQuestStatus : DialogueAction
{
	public int questId;
	public BaseQuest.QuestState state;

	public DialogueSetQuestStatus()
	{

	}

	public DialogueSetQuestStatus(int questId, BaseQuest.QuestState state)
	{
		this.questId = questId;
		this.state = state;
	}

	public override bool Execute(NPC npc)
	{
		BaseQuest q = GameManager.instance.questList.Find(x => x.questId == questId);
		if (q != null)
			q.SetState(state);

		return base.Execute(npc);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();

	}
}
