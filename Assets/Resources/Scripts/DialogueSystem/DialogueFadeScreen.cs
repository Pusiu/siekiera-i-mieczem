using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueFadeScreen : DialogueAction
{
	public bool fadeIn;

	public override bool Execute(NPC npc)
	{
		if (fadeIn)
			GameUI.instance.FadeInRebuildingScreen();
		else
			GameUI.instance.FadeOutRebuildingScreen();

		GameManager.instance.ExecuteAction(() =>
		{
			if (!fadeIn)
				GameUI.instance.timePassOverlay.SetActive(false);

			npc.ProcessLine();
		},3);

		return false;
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Fade in?:");
		fadeIn = EditorGUILayout.Toggle(fadeIn);
		EditorGUILayout.EndHorizontal();
	}
}
