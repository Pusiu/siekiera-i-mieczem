using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueShowGuideEntry : DialogueAction
{
	public string entryName;

	public override bool Execute(NPC npc)
	{
		GameObject entry = null;
		for (int i=0; i < GameUI.instance.guideScreenEntriesContents.transform.childCount; i++)
		{
			if (GameUI.instance.guideScreenEntriesContents.transform.GetChild(i).name == entryName)
			{
				entry = GameUI.instance.guideScreenEntriesContents.transform.GetChild(i).gameObject;
				break;
			}
		}
		if (entry != null)
		{
			GameUI.instance.ShowGuideScreen();
			GameUI.instance.SelectGuideScreenEntry(entry);
		}
		else
		{
			Debug.LogError("[Dialogue]No guide screen entry found: " + entryName);
		}

		return true;
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();
		EditorGUILayout.LabelField("Shows guide screen with given entry");

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Entry name");
		entryName = EditorGUILayout.TextField(entryName);
		EditorGUILayout.EndHorizontal();
	}
}
