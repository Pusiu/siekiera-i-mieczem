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

	public override void Execute(NPC npc)
	{
		base.Execute(npc);
		BaseQuest q = GameManager.instance.questList.Find(x => x.questId == questId);
		if (q != null)
			q.SetState(state);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();
		questId = EditorGUILayout.IntField(questId, new GUILayoutOption[] { GUILayout.MaxWidth(100) });
		
		state = (BaseQuest.QuestState)EditorGUILayout.EnumPopup(state);
	}
}
