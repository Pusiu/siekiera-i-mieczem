using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : Resource //MonoBehaviour, IInteractable
{
	public bool canBePickedUp = false;
	public bool requireTool = true;
	public Tool.ToolType requiredToolType;
	//public Resource.ResourceType resourceType;
	public int count = 1;

	public virtual void Gather()
	{
		if (requireTool && PlayerController.instance.GetToolByType(requiredToolType) == null)
			GameUI.instance.ShowHint("Nie masz wymaganego narzędzia!");

		//Debug.LogWarning("Resource generator hasn't specified gather method");
	}

	public override void OnInteraction()
	{
		GameObject target = gameObject;
		if (isOnCart)
		{
			target = Cart.instance.loadingPos.gameObject;
		}

		PlayerController.instance.MoveTo(target);
		PlayerController.instance.OnTargetReached += (args) =>
		{
			if (args != target)
				return;

			PlayerController.instance.ClearOnTargetReachedListeners();

			if (canBePickedUp)
			{
				if (requireTool && PlayerController.instance.GetToolByType(requiredToolType) != null)
				{
					return;
				}
				Pickup();
			}
			Gather();
		};
	}

}
