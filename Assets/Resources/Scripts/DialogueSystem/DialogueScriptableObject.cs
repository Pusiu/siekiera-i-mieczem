using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="dialogue", menuName ="Scriptable Objects/Dialogue")]
[System.Serializable]
public class DialogueScriptableObject : ScriptableObject
{
	public List<DialogueAction> lines = new List<DialogueAction>();
}
