using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
	public DialogueScriptableObject currentDialogue;
	public Animator animator;
	public int currentDialogueLineIndex = 0;

	// Start is called before the first frame update
	void Start()
    {
		animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnInteraction()
	{
		if (currentDialogue != null)
			StartDialogue();
	}



	public void ProcessLine()
	{
		GameUI.instance.speechBubble.gameObject.SetActive(false);
		if (currentDialogueLineIndex >= currentDialogue.lines.Count)
		{
			return;
		}
		currentDialogue.lines[currentDialogueLineIndex].Execute(this);
	}

	public void StartDialogue()
	{
		ProcessLine();
	}
}
