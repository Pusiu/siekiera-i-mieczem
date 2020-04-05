using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundCollider : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<PlayerController>() != null)
		{
			GameUI.instance.currentSpeechFocus = PlayerController.instance.gameObject;
			GameUI.instance.Typewrite("Nie powinienem się oddalać od wioski");
		}
	}
}
