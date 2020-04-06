using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueSwitch : DialogueAction
{
	public bool currentNPC = true;
	public int npcID;

	public bool endCurrent;
	public DialogueScriptableObject nextDialogue;

	public override bool Execute(NPC npc)
	{
		NPC target = npc;
		if (!currentNPC)
			npc = GameManager.instance.npcs.Find(x => x.id == npcID).GetComponent<NPC>();

		if (endCurrent)
		{
			target.SetDialogue(null);
			GameManager.instance.ExecuteAction(() =>
			{
				target.SetDialogue(nextDialogue);
			}, .3f);
		}
		else
			target.SetDialogue(nextDialogue);

		return base.Execute(npc);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Current NPC?:");
		currentNPC = EditorGUILayout.Toggle(currentNPC);
		GUILayout.EndHorizontal();

		if (!currentNPC)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("NPC ID:");
			npcID = EditorGUILayout.IntField(npcID);
			GUILayout.EndHorizontal();
		}

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
