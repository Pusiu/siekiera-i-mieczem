using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : LivingBeing
{
	//public int npcID = -1;
	public DialogueScriptableObject currentDialogue;
	//public Animator animator;
	public int currentDialogueLineIndex = 0;

	// Start is called before the first frame update
	void Start()
    {
		animator = GetComponent<Animator>();
		if (id == -1)
			Debug.LogError($"NPC {name} ID is not set!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public override void OnInteraction()
	{
		PlayerController.instance.movePoint.transform.position = transform.position + transform.forward * 3;
		PlayerController.instance.MoveTo(PlayerController.instance.movePoint);
		PlayerController.instance.OnTargetReached += (args) =>
		{
			PlayerController.instance.ClearOnTargetReachedListeners();
			PlayerController.instance.MoveTo(PlayerController.instance.gameObject);
			PlayerController.instance.transform.LookAtYOnly(transform.position);
			if (currentDialogue != null)
				StartDialogue();
		};
	}

	public void SetDialogue(DialogueScriptableObject d)
	{
		currentDialogueLineIndex = -1;
		currentDialogue = d;
	}

	public void ProcessLine()
	{
		do
		{
			GetComponent<AudioSource>().Stop();
			PlayerController.instance.playerMouth.Stop();
			PlayerController.instance.currentTalkingNPC = this;
			currentDialogueLineIndex++;
			GameUI.instance.speechBubble.gameObject.SetActive(false);
			PlayerController.instance.canMove = false;
			if (currentDialogue == null || currentDialogueLineIndex >= currentDialogue.lines.Count)
			{
				PlayerController.instance.canMove = true;
				PlayerController.instance.currentTalkingNPC = null;
				return;
			}
		} while (currentDialogue.lines[currentDialogueLineIndex].Execute(this));
	}

	public void StartDialogue()
	{
		PlayerController.instance.cameraZoom = 3;
		currentDialogueLineIndex = -1;
		ProcessLine();
	}
}
