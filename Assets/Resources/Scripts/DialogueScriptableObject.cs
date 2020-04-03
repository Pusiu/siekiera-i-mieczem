using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
	public enum Speaker { Player, NPC };
	public Speaker speaker;
	public string text;

	public DialogueLine(Speaker sp, string text)
	{
		this.speaker = sp;
		this.text = text;
	}
}

[CreateAssetMenu(fileName ="dialogue", menuName ="Scriptable Objects/Dialogue")]
[System.Serializable]
public class DialogueScriptableObject : ScriptableObject
{
	public List<DialogueLine> lines = new List<DialogueLine>();
}
