using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(DialogueScriptableObject))]
[ExecuteInEditMode]
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
			o.lines.Insert(index+1, (DialogueAction)ScriptableObject.CreateInstance(typeof(DialogueLine)));
			o.lines[index + 1].name = "DialogueLine";
			AssetDatabase.AddObjectToAsset(o.lines[index+1], o);
			EditorUtility.SetDirty(o.lines[index+1]);
		}
		if (GUILayout.Button("-",new GUILayoutOption[] { GUILayout.MaxWidth(50) }))
		{
			o.lines.RemoveAt(index);
		}
		GUILayout.EndHorizontal();
	}

	public List<Type> types = new List<Type>();
	private void Awake()
	{
		UpdateTypes();
	}

	void UpdateTypes()
	{
		foreach (Type t in System.Reflection.Assembly.Load("Assembly-CSharp").GetTypes())
		{
			if (t.IsSubclassOf(typeof(DialogueAction)))
			{
				types.Add(t);
			}
		}
	}



	public override void OnInspectorGUI()
	{
		if (types.Count == 0)
			UpdateTypes();

		base.OnInspectorGUI();
		o = (DialogueScriptableObject)target;
		for (int i=0; i < o.lines.Count;i++)
		{
			DialogueAction d = o.lines[i];
			if (o.lines[i] == null)
			{
				o.lines.Clear();
				return;
			}
			EditorGUILayout.BeginHorizontal();
			int currentTypeIndex = types.IndexOf(o.lines[i].GetType());
			if (currentTypeIndex >= 0)
			{
				currentTypeIndex = EditorGUILayout.Popup(currentTypeIndex, types.Select(x => x.Name).ToArray());
				if (types[currentTypeIndex] != o.lines[i].GetType())
				{
					o.lines[i] = (DialogueAction)ScriptableObject.CreateInstance(types[currentTypeIndex]); //(DialogueAction)Activator.CreateInstance(types[currentTypeIndex]);
					o.lines[i].name = o.lines[i].GetType().ToString();
					AssetDatabase.AddObjectToAsset(o.lines[i], o);
					EditorUtility.SetDirty(o.lines[i]);
					d = o.lines[i];
				}

				DrawPlusMinus(i);
				d.DrawInspectorLine();
			}
			EditorGUILayout.EndHorizontal();
		}
		DrawPlusMinus(-1);

		EditorUtility.SetDirty(o);
		//AssetDatabase.SaveAssets();
	}
}
