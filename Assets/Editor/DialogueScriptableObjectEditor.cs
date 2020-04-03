using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueScriptableObject))]
public class DialogueScriptableObjectEditor : Editor
{
	DialogueScriptableObject o;
	public void DrawPlusMinus(int index)
	{
		if (index == -1)
			index = o.lines.Count-1;

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.MaxWidth(50) }))
		{
			o.lines.Insert(index+1, new DialogueLine(DialogueLine.Speaker.NPC, ""));
		}
		if (GUILayout.Button("-",new GUILayoutOption[] { GUILayout.MaxWidth(50) }))
		{
			o.lines.RemoveAt(index);
		}
		GUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		o = (DialogueScriptableObject)target;
		for (int i=0; i < o.lines.Count;i++)
		{
			DialogueLine d = o.lines[i];
			EditorGUILayout.BeginHorizontal();
			DrawPlusMinus(i);
			d.speaker = (DialogueLine.Speaker)EditorGUILayout.EnumPopup(d.speaker, new GUILayoutOption[] { GUILayout.MaxWidth(100)});
			EditorStyles.textArea.wordWrap = true;
			d.text = GUILayout.TextArea(d.text, new GUILayoutOption[] { GUILayout.MinWidth(200), GUILayout.MinHeight(200), GUILayout.ExpandWidth(false)});
			EditorGUILayout.EndHorizontal();
		}
		EditorUtility.SetDirty(o);
		DrawPlusMinus(-1);
	}
}
