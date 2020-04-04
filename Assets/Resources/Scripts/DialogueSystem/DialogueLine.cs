using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DialogueLine : DialogueAction
{
	public enum Speaker { Player, NPC };
	public Speaker speaker;
	public string text;

	public DialogueLine() { }

	public DialogueLine(Speaker sp, string text)
	{
		this.speaker = sp;
		this.text = text;
	}

	public override bool Execute(NPC npc)
	{
		//base.Execute(npc);


		bool isNPCSpeaking = (speaker == Speaker.NPC) ? true : false;

		Animator a = isNPCSpeaking ? npc.animator : PlayerController.instance.animator;
		a.SetTrigger("TalkTrigger");

		GameUI.instance.currentSpeechFocus = isNPCSpeaking ? npc.gameObject : PlayerController.instance.gameObject;

		GameManager.instance.ExecuteAction(() =>
		{
			npc.ProcessLine();
		}, text.Length * GameUI.instance.typewriteLetterTime + 1);
		GameUI.instance.Typewrite(text);

		return false;
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Speaker:");
		speaker = (DialogueLine.Speaker)EditorGUILayout.EnumPopup(speaker);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Text:");
		EditorStyles.textArea.wordWrap = true;
		text = GUILayout.TextArea(text, new GUILayoutOption[] { GUILayout.MinWidth(200), GUILayout.MinHeight(200)});
		EditorGUILayout.EndHorizontal();

	}
}
