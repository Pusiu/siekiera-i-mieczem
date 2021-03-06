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

	NPC executingNPC;
	AudioSource src;
	public override bool Execute(NPC npc)
	{
		//base.Execute(npc);
		executingNPC = npc;

		bool isNPCSpeaking = (speaker == Speaker.NPC) ? true : false;

		Animator a = isNPCSpeaking ? npc.animator : PlayerController.instance.animator;
		src = isNPCSpeaking ? npc.GetComponent<AudioSource>() : PlayerController.instance.playerMouth;
		a.SetTrigger("TalkTrigger");

		GameUI.instance.currentSpeechFocus = isNPCSpeaking ? npc.gameObject : PlayerController.instance.gameObject;
		src.Play();

		GameUI.instance.OnTypewriteEnded += OnTypewriteEnded;

		GameUI.instance.Typewrite(text);

		return false;
	}

	private void OnTypewriteEnded()
	{
		GameUI.instance.OnTypewriteEnded -= OnTypewriteEnded;
		src.Stop();
		GameManager.instance.ExecuteAction(() =>
		{
			if (executingNPC != null && 
				executingNPC.currentDialogue != null && 
				executingNPC.currentDialogueLineIndex < executingNPC.currentDialogue.lines.Count &&
				executingNPC.currentDialogueLineIndex >= 0 &&
				executingNPC.currentDialogue.lines[executingNPC.currentDialogueLineIndex] == this)
				executingNPC.ProcessLine();
		}, 2);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();

	}
}
