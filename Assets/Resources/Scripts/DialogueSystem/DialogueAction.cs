using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueAction : ScriptableObject
{
	/// <summary>
	/// Executes dialogue action
	/// By default, returns true, meaning it will immediately proceed to next line
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
