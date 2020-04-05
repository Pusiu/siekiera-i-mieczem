using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueHasResource : DialogueAction
{
	public Resource.ResourceType t;
	public int count = 1;
	public bool exitIfFalse = true;

	public override bool Execute(NPC npc)
	{
		if (PlayerController.instance.HasResource(t,count))
		{
			return true;
		}
		else
		{
			if (exitIfFalse)
			{
				npc.currentDialogueLineIndex = npc.currentDialogue.lines.Count + 1;
			}

			return true;
		}
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Resource type:");
		t = (Resource.ResourceType)EditorGUILayout.EnumPopup(t);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Count:");
		count = EditorGUILayout.IntField(count);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Exit if false:");
		exitIfFalse = EditorGUILayout.Toggle(exitIfFalse);
		EditorGUILayout.EndHorizontal();
	}
}
