using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueSwitch : DialogueAction
{
	public bool currentNPC = true;
	public int npcID;

	public bool endCurrent;
	public DialogueScriptableObject nextDialogue;

	public override bool Execute(NPC npc)
	{
		NPC target = npc;
		if (!currentNPC)
			npc = GameManager.instance.npcs.Find(x => x.id == npcID).GetComponent<NPC>();

		if (endCurrent)
		{
			target.SetDialogue(null);
			GameManager.instance.ExecuteAction(() =>
			{
				target.SetDialogue(nextDialogue);
			}, .3f);
		}
		else
			target.SetDialogue(nextDialogue);

		return base.Execute(npc);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();


	}
}
