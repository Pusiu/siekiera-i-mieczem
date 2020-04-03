using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider))]
public class ResourceArea : MonoBehaviour
{
	BoxCollider col;

	public Dictionary<Resource.ResourceType, int> resourcesCount = new Dictionary<Resource.ResourceType, int>();

	public delegate void OnResourceAreaUpdate();
	public event OnResourceAreaUpdate OnResourceAreaUpdateEvent;

	private void Awake()
	{
		resourcesCount.Add(Resource.ResourceType.Stone, 0);
		resourcesCount.Add(Resource.ResourceType.Wood, 0);
		col = GetComponent<BoxCollider>();
	}


	public void CalculateResourcesInside()
	{
		foreach (Resource.ResourceType t in resourcesCount.Keys.ToList())
			resourcesCount[t] = 0;
			
		Collider[] cols = Physics.OverlapBox(transform.position, col.size / 2, col.transform.rotation);
		foreach (Collider c in cols)
		{
			Resource r = c.GetComponentInChildren<Resource>();
			if (r!=null)
			{
				resourcesCount[r.resourceType]++;
			}
			else
			{
				ResourceGenerator rg = c.GetComponentInChildren<ResourceGenerator>();
				if (rg != null)
					resourcesCount[rg.resourceType] += rg.count;
			}
		}
		OnResourceAreaUpdateEvent();
	}

	public void RemoveResources()
	{
		Collider[] cols = Physics.OverlapBox(transform.position, col.size / 2, col.transform.rotation);

		foreach (Resource.ResourceType t in resourcesCount.Keys)
		{
			//Remove from player
			Resource lh = PlayerController.instance.hands[PlayerController.Hand.Left]?.GetComponent<Resource>();
			if (lh != null && lh.resourceType == t)
			{
				Resource rh = PlayerController.instance.hands[PlayerController.Hand.Right]?.GetComponent<Resource>();
				if (rh != null && rh.resourceType == t)
					PlayerController.instance.hands[PlayerController.Hand.Right] = null;

				Destroy(lh.gameObject);
				PlayerController.instance.hands[PlayerController.Hand.Left] = null;
			}
			for (int i=0; i < PlayerController.instance.items.Count;i++)
			{
				Item it = PlayerController.instance.items[i];
				Resource r = it?.GetComponent<Resource>();
				if ( r!= null && r.resourceType == t)
				{
					PlayerController.instance.items[i] = null;
					Destroy(it.gameObject);
				}
			}

			//Remove from cart
			List<Transform> arr = t == Resource.ResourceType.Wood ? Cart.instance.treesPos : Cart.instance.stonesPos;
			foreach (Transform tr in arr)
			{
				if (tr.childCount > 0)
					Destroy(tr.GetChild(0).gameObject);
			}

			//Remove from ground
			List<Collider> l = cols.Where(x => x.GetComponentInChildren<Resource>()?.resourceType == t).ToList();
			l.ForEach(x => Destroy(x.gameObject));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Invoke("CalculateResourcesInside", 2);
	}
	private void OnTriggerExit(Collider other)
	{
		Invoke("CalculateResourcesInside", 2);
	}
}
