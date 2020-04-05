using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueHasTool : DialogueAction
{
	public Tool.ToolType t;
	public bool exitIfFalse = true;

	public override bool Execute(NPC npc)
	{
		if (PlayerController.instance.GetToolByType(t) != null)
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
		EditorGUILayout.LabelField("Tool type:");
		t = (Tool.ToolType)EditorGUILayout.EnumPopup(t);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Exit if false:");
		exitIfFalse = EditorGUILayout.Toggle(exitIfFalse);
		EditorGUILayout.EndHorizontal();
	}
}
