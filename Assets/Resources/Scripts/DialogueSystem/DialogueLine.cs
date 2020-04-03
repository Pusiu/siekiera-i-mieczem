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

	public override void Execute(NPC npc)
	{
		base.Execute(npc);


		bool isNPCSpeaking = (speaker == Speaker.NPC) ? true : false;

		Animator a = isNPCSpeaking ? npc.animator : PlayerController.instance.animator;
		a.SetTrigger("TalkTrigger");

		GameUI.instance.currentSpeechFocus = isNPCSpeaking ? npc.gameObject : PlayerController.instance.gameObject;

		GameManager.instance.ExecuteAction(() =>
		{
			npc.currentDialogueLineIndex++;
			npc.ProcessLine();
		}, text.Length * GameUI.instance.typewriteLetterTime + 1);
		GameUI.instance.Typewrite(text);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();
		speaker = (DialogueLine.Speaker)EditorGUILayout.EnumPopup(speaker, new GUILayoutOption[] { GUILayout.MaxWidth(100) });
		EditorStyles.textArea.wordWrap = true;
		text = GUILayout.TextArea(text, new GUILayoutOption[] { GUILayout.MinWidth(200), GUILayout.MinHeight(200), GUILayout.ExpandWidth(false) });
	}
}
