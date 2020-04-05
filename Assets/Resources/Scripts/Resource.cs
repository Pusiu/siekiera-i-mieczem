using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : PickableObject
{
    public enum ResourceType { Stone, Wood, Flower, Anvil};
	public ResourceType resourceType;
	public bool isOnCart = false;

	public override void OnInteraction()
	{
		base.OnInteraction();
		if (isOnCart)
		{
			PlayerController.instance.agent.SetDestination(Cart.instance.loadingPos.position);
		}
	}

	public override bool Pickup()
	{
		if (base.Pickup())
		{
			if (isOnCart)
				isOnCart = false;

			return true;
		}
		return false;
	}
}
