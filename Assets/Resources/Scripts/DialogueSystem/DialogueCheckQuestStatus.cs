using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueCheckQuestStatus : DialogueAction
{
	public int questID;
	public BaseQuest.QuestState state;

	public override bool Execute(NPC npc)
	{
		BaseQuest q = GameManager.instance.questList.Find(x => x.questId == questID);
		if (q.questState == state)
			return true;

		return false;
	}

	public override void DrawInspectorLine()
	{

	}
}
