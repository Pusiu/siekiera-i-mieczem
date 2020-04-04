using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{

	public void OnInteraction()
	{
		PlayerController.instance.canMove = false;
		GameUI.instance.FadeInRebuildingScreen();
		GameManager.instance.ExecuteAction(() =>
		{
			GameManager.instance.GetComponent<TimeAndWeather>().SetTime(700);
			PlayerController.instance.energy = 100;
			PlayerController.instance.canMove = true;
			GameUI.instance.FadeOutRebuildingScreen();
		},3);
	}
}
