using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundCollider : MonoBehaviour
{

	public static void StopPlayer()
	{
		PlayerController.instance.MoveTo(PlayerController.instance.gameObject);
		GameUI.instance.currentSpeechFocus = PlayerController.instance.gameObject;
		GameUI.instance.Typewrite("Nie powinienem się oddalać od wioski");
	}

	private void OnMouseDown()
	{
		StopPlayer();
	}
}
