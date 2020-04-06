using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueSetNPCState : DialogueAction
{
	public int npcID;
	public bool active;

	public override bool Execute(NPC npc)
	{
		LivingBeing target = GameManager.instance.npcs.Find(x => x.id == npcID);
		if (target != null)
			target.gameObject.SetActive(active);

		return base.Execute(npc);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Targeted NPC ID:");
		npcID = EditorGUILayout.IntField(npcID);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Set active?");
		active = EditorGUILayout.Toggle(active);
		EditorGUILayout.EndHorizontal();
	}
}

