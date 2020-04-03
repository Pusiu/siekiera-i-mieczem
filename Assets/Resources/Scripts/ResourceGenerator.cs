using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour, IInteractable
{
	public bool requireTool = true;
	public Tool.ToolType requiredToolType;



	public virtual void OnInteraction()
	{
		PlayerController.instance.MoveTo(gameObject);
	}

}
