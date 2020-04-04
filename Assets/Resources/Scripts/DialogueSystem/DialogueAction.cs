using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueAction : ScriptableObject
{
	/// <summary>
	/// Executes dialogue action
	/// </summary>
	/// <param name="npc"></param>
	/// <returns>true if npc should immediately proceed to next line</returns>
	public virtual bool Execute(NPC npc)
	{
		return true;
	}

	public virtual void DrawInspectorLine()
	{

	}
}
