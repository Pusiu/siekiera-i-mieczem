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
			int currentTypeIndex = types.IndexOf(o.lines[i].GetType());
			if (currentTypeIndex >= 0)
			{
				EditorGUILayout.BeginHorizontal();
				currentTypeIndex = EditorGUILayout.Popup(currentTypeIndex, types.Select(x => x.Name).ToArray(), new GUILayoutOption[] { GUILayout.MinWidth(150)});
				if (types[currentTypeIndex] != o.lines[i].GetType())
				{
					o.lines[i] = (DialogueAction)ScriptableObject.CreateInstance(types[currentTypeIndex]); //(DialogueAction)Activator.CreateInstance(types[currentTypeIndex]);
					o.lines[i].name = o.lines[i].GetType().ToString();
					AssetDatabase.AddObjectToAsset(o.lines[i], o);
					EditorUtility.SetDirty(o.lines[i]);
					d = o.lines[i];
				}

				DrawPlusMinus(i);
				EditorGUILayout.EndHorizontal();

				//d.DrawInspectorLine();
				DrawInspectorLine(d);
			}
			Rect rect = EditorGUILayout.GetControlRect(false, 1);

			rect.height = 1;

			EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
		}
		DrawPlusMinus(-1);

		EditorUtility.SetDirty(o);
		//AssetDatabase.SaveAssets();
	}

	public void DrawInspectorLine(DialogueAction action)
	{
		if (action is DialogueCheckQuestStatus)
		{
			DialogueCheckQuestStatus c = (DialogueCheckQuestStatus)action;
			EditorStyles.label.wordWrap = true;
			EditorGUILayout.LabelField("This action let's dialogue proceed only if given quest status evaluates to true", new GUILayoutOption[] { GUILayout.MinHeight(50) });

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest ID:");
			c.questID = EditorGUILayout.IntField(c.questID);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest state:");
			c.state= (BaseQuest.QuestState)EditorGUILayout.EnumPopup(c.state);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueFadeScreen)
		{
			DialogueFadeScreen f = (DialogueFadeScreen)action;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Fade in?:");
			f.fadeIn = EditorGUILayout.Toggle(f.fadeIn);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueHasResource)
		{
			DialogueHasResource d = (DialogueHasResource)action;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Resource type:");
			d.t = (Resource.ResourceType)EditorGUILayout.EnumPopup(d.t);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Count:");
			d.count = EditorGUILayout.IntField(d.count);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Exit if false:");
			d.exitIfFalse = EditorGUILayout.Toggle(d.exitIfFalse);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueHasTool)
		{
			DialogueHasTool d = (DialogueHasTool)action;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Tool type:");
			d.t = (Tool.ToolType)EditorGUILayout.EnumPopup(d.t);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Exit if false:");
			d.exitIfFalse = EditorGUILayout.Toggle(d.exitIfFalse);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueLine)
		{
			DialogueLine d = (DialogueLine)action;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Speaker:");
			d.speaker = (DialogueLine.Speaker)EditorGUILayout.EnumPopup(d.speaker);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Text:");
			EditorStyles.textArea.wordWrap = true;
			d.text = GUILayout.TextArea(d.text, new GUILayoutOption[] { GUILayout.MinWidth(200), GUILayout.MinHeight(200) });
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueRemoveResource)
		{
			DialogueRemoveResource d = (DialogueRemoveResource)action;
			EditorGUILayout.LabelField("Removes x resources from player's inventory");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Resource type:");
			d.t = (Resource.ResourceType)EditorGUILayout.EnumPopup(d.t);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Count:");
			d.count = EditorGUILayout.IntField(d.count);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueSetNPCState)
		{
			DialogueSetNPCState d = (DialogueSetNPCState)action;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Targeted NPC ID:");
			d.npcID = EditorGUILayout.IntField(d.npcID);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Set active?");
			d.active = EditorGUILayout.Toggle(d.active);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueSetQuestStatus)
		{
			DialogueSetQuestStatus d = (DialogueSetQuestStatus)action;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest ID:");
			d.questId = EditorGUILayout.IntField(d.questId, new GUILayoutOption[] { GUILayout.MaxWidth(100) });
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest status:");
			d.state = (BaseQuest.QuestState)EditorGUILayout.EnumPopup(d.state);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueSetTime)
		{
			DialogueSetTime d = (DialogueSetTime)action;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Time:");
			d.time = EditorGUILayout.IntField(d.time);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueShowGuideEntry)
		{
			DialogueShowGuideEntry d = (DialogueShowGuideEntry)action;
			EditorGUILayout.LabelField("Shows guide screen with given entry");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Entry name");
			d.entryName = EditorGUILayout.TextField(d.entryName);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueSpawnTool)
		{
			DialogueSpawnTool d = (DialogueSpawnTool)action;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Tool prefab:");
			d.toolPrefab = (GameObject)EditorGUILayout.ObjectField(d.toolPrefab, typeof(GameObject), false);
			EditorGUILayout.EndHorizontal();
		}
		else if (action is DialogueSwitch)
		{
			DialogueSwitch d = (DialogueSwitch)action;

			GUILayout.BeginHorizontal();
			GUILayout.Label("Current NPC?:");
			d.currentNPC = EditorGUILayout.Toggle(d.currentNPC);
			GUILayout.EndHorizontal();

			if (!d.currentNPC)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("NPC ID:");
				d.npcID = EditorGUILayout.IntField(d.npcID);
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Next dialogue:");
			d.nextDialogue = (DialogueScriptableObject)EditorGUILayout.ObjectField(d.nextDialogue, typeof(DialogueScriptableObject), false, new GUILayoutOption[] { GUILayout.MinWidth(200) });
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("End current dialogue:");
			d.endCurrent = EditorGUILayout.Toggle(d.endCurrent);
			GUILayout.EndHorizontal();
		}
		else if (action is DialogueWait)
		{
			DialogueWait d = (DialogueWait)action;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Wait time:");
			d.time = EditorGUILayout.FloatField(d.time);
			EditorGUILayout.EndHorizontal();
		}
	}
}
