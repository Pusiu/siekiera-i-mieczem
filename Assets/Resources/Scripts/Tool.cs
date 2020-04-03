using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : PickableObject, IInteractable
{
	public enum ToolType { Axe, Pickaxe, Sword};
	public ToolType toolType;

	public enum ToolStatus { Unequiped, Holstered, InHand};

	public ToolStatus toolstatus = ToolStatus.Unequiped;

	// Start is called before the first frame update
	void Start()
    {
		handlingMethod = HandlingMethod.InOneHand;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public override void PutDown()
	{
		base.PutDown();
		toolstatus = ToolStatus.Unequiped;
	}

	public override bool Pickup()
	{
		if (toolstatus != ToolStatus.Unequiped)
			return false;

		PlayerController.Hand? freehand = PlayerController.instance.GetFreeHand();
		bool r = base.Pickup();
		if (r)
		{
			GameManager.instance.ExecuteAction(() =>
			{
				if (freehand.Value == PlayerController.Hand.Left)
				{
					transform.localPosition = PlayerController.instance.leftinHandOffsetPosition;
					transform.localRotation = Quaternion.Euler(PlayerController.instance.leftinHandOffsetRotation);
				}
				else
				{
					transform.localPosition = PlayerController.instance.rightinHandOffsetPosition;
					transform.localRotation = Quaternion.Euler(PlayerController.instance.rightinHandOffsetRotation);
				}
				toolstatus = ToolStatus.InHand;
			}, 0.6f);
		}

		return r;
	}
}
