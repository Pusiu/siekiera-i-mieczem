using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class ResourceArea : MonoBehaviour
{
	public Text stoneText;
	public Text woodText;

	BoxCollider col;

	public Dictionary<Resource.ResourceType, int> resourcesCount = new Dictionary<Resource.ResourceType, int>();

	public delegate void OnResourceAreaUpdate();
	public event OnResourceAreaUpdate OnResourceAreaUpdateEvent;

	private void Awake()
	{
		resourcesCount.Add(Resource.ResourceType.Rock, 0);
		resourcesCount.Add(Resource.ResourceType.Wood, 0);
		col = GetComponent<BoxCollider>();
	}

	private void Start()
	{
		gameObject.SetActive(false);
	}


	public void CalculateResourcesInside()
	{
		foreach (Resource.ResourceType t in resourcesCount.Keys.ToList())
			resourcesCount[t] = 0;


			
		Collider[] cols = Physics.OverlapBox(transform.position+col.center, Vector3.Scale(transform.localScale, col.size / 2), col.transform.rotation);
		foreach (Collider c in cols)
		{
			/*ResourceGenerator rg = c.GetComponentInChildren<ResourceGenerator>();
			if (rg != null)
			{
				resourcesCount[rg.resourceType] += rg.count;
			}
			else
			{*/
				Resource r = c.GetComponent<Resource>();
				if (r!=null)
					if (resourcesCount.ContainsKey(r.resourceType))
						resourcesCount[r.resourceType]++;
			//}
		}
		OnResourceAreaUpdateEvent();
	}

	public void SetResourceText(int maxWood, int maxStone)
	{
		stoneText.text = $"{resourcesCount[Resource.ResourceType.Rock].ToString()}/{maxStone.ToString()}";
		woodText.text = $"{resourcesCount[Resource.ResourceType.Wood].ToString()}/{maxWood.ToString()}";
	}

	public void RemoveResources()
	{
		Collider[] cols = Physics.OverlapBox(transform.position + col.center, Vector3.Scale(transform.localScale, col.size / 2), col.transform.rotation);

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
				PlayerController.instance.animator.SetTrigger("Pickup");
			}
			for (int i=0; i < PlayerController.instance.items.Count;i++)
			{
				Item it = PlayerController.instance.items[i];
				if (it == null)
					continue;

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
			List<Collider> l = cols.Where(x => x.GetComponent<Resource>()?.resourceType == t).ToList();
			l.ForEach(x => Destroy(x.gameObject));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Invoke("CalculateResourcesInside", 2);
	}
	private void OnTriggerStay(Collider other)
	{
		CalculateResourcesInside();	
	}
	private void OnTriggerExit(Collider other)
	{
		Invoke("CalculateResourcesInside", 2);
	}
}
