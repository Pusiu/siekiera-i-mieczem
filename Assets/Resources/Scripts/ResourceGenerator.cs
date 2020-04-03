using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour, IInteractable
{
	public bool requireTool = true;
	public Tool.ToolType requiredToolType;
	public Resource.ResourceType resourceType;
	public int count = 1;


	public virtual void OnInteraction()
	{
		PlayerController.instance.MoveTo(gameObject);
	}

}
