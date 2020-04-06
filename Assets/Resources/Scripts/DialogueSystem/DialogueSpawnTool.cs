using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueSpawnTool : DialogueAction
{
	public GameObject toolPrefab;

	public override bool Execute(NPC npc)
	{
		Vector3 pos = (PlayerController.instance.transform.position - npc.transform.position) / 2;
		Instantiate(toolPrefab, npc.transform.position+pos+Vector3.up, Quaternion.identity);
		return base.Execute(npc);
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();


	}
}
