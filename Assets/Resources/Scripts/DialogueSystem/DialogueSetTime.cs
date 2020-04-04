using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueSetTime : DialogueAction
{
	public int time;

	public override bool Execute(NPC npc)
	{
		//return base.Execute(npc);
		GameManager.instance.GetComponent<TimeAndWeather>().SetTime(time);

		return true;
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Time:");
		time = EditorGUILayout.IntField(time);
		EditorGUILayout.EndHorizontal();
	}
}
