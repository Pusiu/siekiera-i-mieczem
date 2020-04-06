using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueWait : DialogueAction
{
	public float time;

	public override bool Execute(NPC npc)
	{
		GameManager.instance.ExecuteAction(() =>
		{
			npc.ProcessLine();
		},time);
		return false;
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();

	}
}
