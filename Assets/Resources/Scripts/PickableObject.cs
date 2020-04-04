using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PickableObject : Item, IInteractable
{

	public virtual void OnInteraction()
	{
		PlayerController.instance.MoveTo(gameObject);
		PlayerController.instance.OnTargetReached += (args) => { Pickup(); };
	}

	public virtual void PutDown()
	{
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().isKinematic = false;

		if (GetComponent<NavMeshObstacle>() != null)
			GetComponent<NavMeshObstacle>().enabled = true;
	}

	/// <summary>
	/// Tries to pickup the object 
	/// </summary>
	/// <returns>True if player can pick the object, false otherwise</returns>
	public virtual bool Pickup()
	{
		PlayerController.instance.interactionTarget = null;

		/*if (!PlayerController.instance.PutItemToHands(this))
			return false;*/

		if (!PlayerController.instance.HasFreeHand((handlingMethod == HandlingMethod.InOneHand) ? PlayerController.Hand.Any : PlayerController.Hand.Both))
			return false;

		if (GetComponent<Rigidbody>() != null)
			GetComponent<Rigidbody>().isKinematic = true;

		if (GetComponent<NavMeshObstacle>() != null)
			GetComponent<NavMeshObstacle>().enabled = false;

		if (handlingMethod != HandlingMethod.InOneHand)
		{
			if (handlingMethod == HandlingMethod.InTwoHands)
			{
				PlayerController.instance.animator.SetBool("Shoulder", false);
				PlayerController.instance.animator.SetBool("LeftHand", true);
				PlayerController.instance.animator.SetBool("RightHand", true);
			}
			else
			{
				PlayerController.instance.animator.SetBool("Shoulder", true);
				PlayerController.instance.animator.SetBool("LeftHand", false);
				PlayerController.instance.animator.SetBool("RightHand", false);
			}

			PlayerController.instance.animator.SetTrigger("Pickup");

			GameManager.instance.ExecuteAction(() =>
			{
				PlayerController.instance.PutItemToHands(this, PlayerController.Hand.Both);
			},0.5f);
			return true;
		}
		else
		{
			PlayerController.instance.animator.SetBool("Shoulder", false);
			PlayerController.instance.animator.SetBool("LeftHand", false);
			PlayerController.instance.animator.SetBool("RightHand", false);
			if (PlayerController.instance.HasFreeHand(PlayerController.Hand.Any))
			{
				GameManager.instance.ExecuteAction(() =>
				{
					PlayerController.instance.animator.SetTrigger("Pickup");
					PlayerController.instance.PutItemToHands(this);
				}, 0.5f);
				return true;
			}
			return false;
		}
	}
}
