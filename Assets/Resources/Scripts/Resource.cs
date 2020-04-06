using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : PickableObject
{
    public enum ResourceType { Branch, Stone, Wood, Flower, Anvil, Rock};
	public ResourceType resourceType;
	public bool isOnCart = false;

	public override void OnInteraction()
	{
		if (isOnCart)
		{
			//PlayerController.instance.agent.SetDestination(.position);
			PlayerController.instance.MoveTo(Cart.instance.loadingPos.gameObject);
			PlayerController.instance.OnTargetReached += (args) => { Pickup(); };
		}
		else
			base.OnInteraction();
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
