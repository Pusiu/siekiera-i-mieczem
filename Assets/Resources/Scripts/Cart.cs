using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cart : Item, IInteractable
{
	public static Cart instance;

	public List<Rigidbody> wheels = new List<Rigidbody>();
	Rigidbody rb;
	public Transform drivingPos;
	public Transform loadingPos;
	public bool isBeingUsed = false;

	public List<Transform> treesPos = new List<Transform>();
	public List<Transform> stonesPos = new List<Transform>();


	public bool firstUse = true;
	void Awake()
	{
		instance = this;
		rb = GetComponent<Rigidbody>();
	}

	public Transform GetFreePlaceForResource(Resource.ResourceType t)
	{
		List<Transform> l = null;
		List<Transform> others = null;
		switch (t)
		{
			case Resource.ResourceType.Anvil:
			case Resource.ResourceType.Rock:
				l = stonesPos;
				others = treesPos;
				break;
			case Resource.ResourceType.Wood:
				l = treesPos;
				others = stonesPos;
				break;
		}

		//don't allow storing stone when there are logs or vice versa
		foreach (Transform it in others)
		{
			if (it.childCount > 0)
				return null;
		}

		foreach (Transform it in l)
		{
			if (it.childCount == 0)
				return it;
		}

		return null;
	}

	public void SetRigidbodiesKinematic(bool c)
	{
		foreach (Rigidbody r in wheels)
			r.isKinematic = c;

		rb.isKinematic = c;
	}

	public void OnInteraction()
	{
		if (firstUse)
		{
			GameObject entry = null;
			for (int i = 0; i < GameUI.instance.guideScreenEntriesContents.transform.childCount; i++)
			{
				if (GameUI.instance.guideScreenEntriesContents.transform.GetChild(i).name == "Wóz")
				{
					entry = GameUI.instance.guideScreenEntriesContents.transform.GetChild(i).gameObject;
					break;
				}
			}
			if (entry != null)
			{
				GameUI.instance.ShowGuideScreen();
				GameUI.instance.SelectGuideScreenEntry(entry);
			}
			firstUse = false;
		}

		if (!isBeingUsed)
		{
			Transform target = drivingPos;
			if (!PlayerController.instance.HasFreeHand(PlayerController.Hand.Both))
			{
				if (PlayerController.instance.hands[PlayerController.Hand.Left].GetComponent<Resource>() != null)
				{
					target = loadingPos;
				}
			}

			PlayerController.instance.MoveTo(target.gameObject);
			PlayerController.instance.OnTargetReached += (args) =>
			 {
				 if (args != target.gameObject)
					 return;

				 PlayerController.instance.MoveTo(PlayerController.instance.gameObject);

				 if (PlayerController.instance.HasFreeHand(PlayerController.Hand.Both) || PlayerController.instance.hands[PlayerController.Hand.Left] == this)
				 {
					 PlayerController.instance.hands[PlayerController.Hand.Left] = this;
					 PlayerController.instance.hands[PlayerController.Hand.Right] = this;
					 PlayerController.instance.transform.position = drivingPos.position;
					 PlayerController.instance.transform.rotation = drivingPos.rotation;
					 GetComponentInChildren<Joint>().connectedBody = PlayerController.instance.GetComponent<Rigidbody>();
					 PlayerController.instance.agent.acceleration = PlayerController.instance.speed;
					 PlayerController.instance.agent.angularSpeed = 40;
					 SetRigidbodiesKinematic(false);
					 isBeingUsed = true;
					PlayerController.instance.animator.SetTrigger("CartTrigger");
				 }
				 else
				 {
					 Resource r = PlayerController.instance.hands[PlayerController.Hand.Left]?.GetComponent<Resource>();
					 if (r == null)
						 return;

					 if (r.handlingMethod == HandlingMethod.InOneHand)
					 {
						 GameUI.instance.ShowHint("Na wóz możesz ładować tylko duże rzeczy");
						 return;
					 }

					 Transform place = GetFreePlaceForResource(r.resourceType);
					 if (place != null)
					 {
						 PlayerController.instance.PutDownHeldObject(place.gameObject);
						 if (r.GetComponentInChildren<Rigidbody>())
							 r.GetComponentInChildren<Rigidbody>().isKinematic = true;

						 if (r.GetComponentInChildren<Collider>())
							 r.GetComponentInChildren<Collider>().isTrigger = true;

						 r.transform.SetParent(place);
						 r.transform.localPosition = Vector3.zero;
						 r.transform.localRotation = Quaternion.identity;
						 r.isOnCart = true;
					 }
					 else
					 {
						 GameUI.instance.ShowHint("Na wozie nie ma na to miejsca");
						 return;
					 }
				 }
			 };
		}
		else
		{
			PlayerController.instance.hands[PlayerController.Hand.Left] = null;
			PlayerController.instance.hands[PlayerController.Hand.Right] = null;
			SetRigidbodiesKinematic(true);
			GetComponentInChildren<Joint>().connectedBody = null;
			PlayerController.instance.agent.acceleration = PlayerController.instance.speed * 2;
			PlayerController.instance.agent.angularSpeed = 120;
			isBeingUsed = false;
			PlayerController.instance.animator.SetTrigger("CartTrigger");
		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (isBeingUsed)
		{
			//GetComponent<NavMeshAgent>().SetDestination(PlayerController.instance.transform.position);
		}
	}
}