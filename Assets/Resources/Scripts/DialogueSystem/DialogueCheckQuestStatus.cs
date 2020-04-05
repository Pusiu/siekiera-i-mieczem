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
		EditorStyles.label.wordWrap = true;
		EditorGUILayout.LabelField("This action let's dialogue proceed only if given quest status evaluates to true", new GUILayoutOption[] { GUILayout.MinHeight(50)});

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Quest ID:");
		questID= EditorGUILayout.IntField(questID);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Quest state:");
		state = (BaseQuest.QuestState)EditorGUILayout.EnumPopup(state);
		EditorGUILayout.EndHorizontal();
	}
}
