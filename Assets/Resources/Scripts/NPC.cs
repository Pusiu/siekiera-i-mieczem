using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
	public DialogueScriptableObject currentDialogue;
	Animator animator;

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

	public void SayLine(int index)
	{
		if (index >= currentDialogue.lines.Count)
		{
			GameUI.instance.speechBubble.gameObject.SetActive(false);
			return;
		}

		bool isNPCSpeaking = (currentDialogue.lines[index].speaker == DialogueLine.Speaker.NPC) ? true : false;

		Animator a =  isNPCSpeaking ? animator : PlayerController.instance.animator;
		a.SetTrigger("TalkTrigger");

		GameUI.instance.currentSpeechFocus = isNPCSpeaking ? gameObject : PlayerController.instance.gameObject;

		GameManager.instance.ExecuteAction(() =>
		{
			SayLine(index + 1);
		}, currentDialogue.lines[index].text.Length * GameUI.instance.typewriteLetterTime + 1);
		GameUI.instance.Typewrite(currentDialogue.lines[index].text);
	}

	public void StartDialogue()
	{
		SayLine(0);
	}
}
