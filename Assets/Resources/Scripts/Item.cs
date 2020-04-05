using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	public string itemName;
	public Sprite image;

	public enum HandlingMethod { InOneHand, InTwoHands, OnShoulder }
	public HandlingMethod handlingMethod;


	public virtual string GetItemDescription()
	{
		return "";
	}
}
