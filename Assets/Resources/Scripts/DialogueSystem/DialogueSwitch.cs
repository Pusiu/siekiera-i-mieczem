using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueSwitch : DialogueAction
{
	public bool endCurrent;
	public DialogueScriptableObject nextDialogue;

	public override bool Execute(NPC npc)
	{
		if (endCurrent)
		{
			npc.SetDialogue(null);
			GameManager.instance.ExecuteAction(() =>
			{
				npc.SetDialogue(nextDialogue);
			}, .3f);
		}
		else
			npc.SetDialogue(nextDialogue);

		return base.Execute(npc);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Next dialogue:");
		nextDialogue = (DialogueScriptableObject)EditorGUILayout.ObjectField(nextDialogue, typeof(DialogueScriptableObject), false, new GUILayoutOption[] { GUILayout.MinWidth(200)});
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("End current dialogue:");
		endCurrent = EditorGUILayout.Toggle(endCurrent);
		GUILayout.EndHorizontal();
	}
}
