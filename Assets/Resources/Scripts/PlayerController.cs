using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : LivingBeing
{
	public static PlayerController instance;

	public enum Hand { Left, Right, Any, Both};

	//public int health = 100;
	public float energy = 100;

	public bool canMove = true;

	public override void OnInteraction()
	{
		//base.OnInteraction();
	}

	public override void Awake()
	{
		base.Awake();
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

		hands.Add(Hand.Left, null);
		hands.Add(Hand.Right, null);
		holster.Add(Hand.Left, null);
		holster.Add(Hand.Right, null);

		items = new List<Item>();
		for (int i = 0; i < inventoryCapacity; i++)
		{
			items.Add(null);
		}
		agent.speed = speed;

		movePoint = Instantiate(movePoint);
		movePoint.SetActive(false);
	}


	//public Animator animator;
	//public NavMeshAgent agent;
	public AudioSource playerMouth;
	public float speed = 5;
	public float cameraAngle = 30;
	public float angleChangeSpeed = 1;
	public Vector3 cameraOffset = new Vector3(10,10,10);
	public float cameraZoomRange = 5;
	public float cameraZoom = 0;
	public float cameraZoomDamping = 2;
	public Transform bothHandsPosition;
	public Transform rightHandBone;
	public Transform leftHandBone;
	public Transform hipsBone;
	public int inventoryCapacity = 5;
	public List<Item> items;


	public GameObject movePoint;
	public GameObject interactionTarget;

	public delegate void TargetReached(GameObject target);

	private event TargetReached TargetReachedEvent;
	public event TargetReached OnTargetReached
	{
		add
		{
			TargetReachedEvent += value;
			OnTargetReachedHandlers.Add(value);
		}
		remove
		{
			TargetReachedEvent -= value;
			OnTargetReachedHandlers.Remove(value);
		}
	}
	List<TargetReached> OnTargetReachedHandlers = new List<TargetReached>();

	public Dictionary<Hand, Tool> holster = new Dictionary<Hand, Tool>(2);
	public Dictionary<Hand, Item> hands = new Dictionary<Hand, Item>(2);

	public Vector3 rightholsteredOffsetPosition;
	public Vector3 rightholsteredOffsetRotation;

	public Vector3 rightinHandOffsetPosition;
	public Vector3 rightinHandOffsetRotation;

	public Vector3 leftholsteredOffsetPosition;
	public Vector3 leftholsteredOffsetRotation;

	public Vector3 leftinHandOffsetPosition;
	public Vector3 leftinHandOffsetRotation;



	// Update is called once per frame
	void Update()
    {
		if (Input.GetKey(KeyCode.A))
			cameraAngle -= angleChangeSpeed;
		if (Input.GetKey(KeyCode.D))
			cameraAngle += angleChangeSpeed;

		if (cameraAngle >= 360)
			cameraAngle = 0;
		else if (cameraAngle < 0)
			cameraAngle = 359;

		float z = Mathf.Sin(cameraAngle * Mathf.Deg2Rad);
		float x = Mathf.Cos(cameraAngle * Mathf.Deg2Rad);

		if (GameUI.instance != null && !GameUI.instance.guideScreen.activeInHierarchy)
		{
			cameraZoom -= Input.mouseScrollDelta.y * cameraZoomDamping;
			if (cameraZoom > cameraZoomRange)
				cameraZoom = cameraZoomRange;
			if (cameraZoom < 1)
				cameraZoom = 1;
		}

		Camera.main.transform.position = transform.position + new Vector3(x*cameraOffset.x, cameraOffset.y, z*cameraOffset.z)*(cameraZoom);
		Camera.main.transform.LookAt(transform.position);


		if (health > 0 && canMove)
		{
			if (Input.GetMouseButtonDown(0))
			{
					Ray r = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
					RaycastHit hit;
					if (Physics.Raycast(r, out hit))
					{
						if (hit.collider.tag == "Terrain")
						{
							interactionTarget = null;
						//GameObject g = new GameObject("MovePoint");
							movePoint.SetActive(true);
							movePoint.transform.position = hit.point;
							MoveTo(movePoint);
							OnTargetReached += (args) =>
							{
								if (args.Equals(movePoint))
									movePoint.SetActive(false);
							};

						}
						else if (hit.collider.GetComponent<IInteractable>() != null)
						{
							hit.collider.GetComponent<IInteractable>().OnInteraction();
						}

					}
			}

			if (Input.GetMouseButtonDown(1))
			{
				if (!HasFreeHand(Hand.Right) || !HasFreeHand(Hand.Left))
				{
					Ray r = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
					RaycastHit hit;
					if (Physics.Raycast(r, out hit))
					{
						if (hit.collider.tag == "Terrain")
						{
							GameObject g = new GameObject("DropPoint");
							g.transform.position = hit.point;
							MoveTo(g);
							movePoint.SetActive(true);
							movePoint.transform.position = hit.point;
							OnTargetReached += (args) =>
							{
								PutDownHeldObject(args);
								movePoint.SetActive(false);
								Destroy(args);
							};

						}
					}
				}
			}


			if (Input.GetKeyDown(KeyCode.Z))
				DrawTool(Hand.Left);
			if (Input.GetKeyDown(KeyCode.X))
				DrawTool(Hand.Right);

		}



		if (Input.GetKeyDown(KeyCode.Tab))
		{
			GameUI.instance.ToggleInventory();
			canMove = !GameUI.instance.inventoryScreen.activeInHierarchy;
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (GameUI.instance.guideScreen.activeInHierarchy)
				GameUI.instance.HideGuideScreen();
			else
				GameUI.instance.ShowGuideScreen();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!GameUI.instance.menu.activeInHierarchy)
				GameUI.instance.ShowMenu();
			else
				GameUI.instance.HideMenu();
		}

		if (interactionTarget != null)
		{
			agent.SetDestination(interactionTarget.transform.position);
		}

		if (Vector3.Distance(transform.position, agent.destination) < agent.stoppingDistance)
		{
			agent.isStopped = true;
			movePoint.SetActive(false);

			if (interactionTarget != null)
			{
				for (int i = 0; i < OnTargetReachedHandlers.Count; i++)
					if (OnTargetReachedHandlers[i] != null)
					{
						OnTargetReachedHandlers[i].Invoke(interactionTarget);
					}
			}
		}

		animator.SetFloat("Velocity",agent.velocity.magnitude / agent.speed);
	}

	public void MoveTo(GameObject target)
	{
		ClearOnTargetReachedListeners();
		float stoppingdistance = 1;
		/*if (target.GetComponentInChildren<Renderer>() != null)
			stoppingdistance = target.GetComponentInChildren<Renderer>().bounds.size.x;*/

		//agent.stoppingDistance = stoppingdistance;
		interactionTarget = target;
		agent.destination = target.transform.position;
		agent.isStopped = false;
		movePoint.SetActive(true);
		movePoint.transform.position = target.transform.position;
	}

	public override void ReceiveDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			canMove = false;
			animator.SetTrigger("Die");
			GameUI.instance.ShowDeathScreen();
		}
	}

	public void PutDownHeldObject(GameObject location)
	{
		interactionTarget = null;
		animator.SetBool("Shoulder", false);
		animator.SetBool("RightHand", false);
		animator.SetBool("LeftHand", false);

		if (HasFreeHand(Hand.Both))
			return;

		if (hands[Hand.Left] == Cart.instance)
		{
			Cart.instance.OnInteraction();
			return;
		}

		PickableObject obj = null;

		Hand h = Hand.Both;
		if (hands[Hand.Right] != null)
		{
			animator.SetBool("RightHand", true);
			obj = hands[Hand.Right].GetComponent<PickableObject>();
			h = Hand.Right;
		}
		else if (hands[Hand.Left] != null)
		{
			animator.SetBool("LeftHand", true);
			obj = hands[Hand.Left].GetComponent<PickableObject>();
			h = Hand.Left;
		}

		obj.PutDown();
		obj.transform.SetParent(null);
		obj.transform.position = location.transform.position;
		obj.transform.position += new Vector3(0, obj.GetComponentInChildren<BoxCollider>().size.y / 2, 0);
		obj.transform.rotation = Quaternion.LookRotation(transform.right, Vector3.up);


		if (hands[Hand.Left].Equals(hands[Hand.Right]))
		{
			if (hands[Hand.Left].handlingMethod == Item.HandlingMethod.InTwoHands)
			{
				animator.SetBool("RightHand", true);
				animator.SetBool("LeftHand", true);
				animator.SetBool("Shoulder", false);
			}
			else
			{
				animator.SetBool("RightHand", false);
				animator.SetBool("LeftHand", false);
				animator.SetBool("Shoulder", true);
			}

			hands[Hand.Left] = null;
		}
		hands[h] = null;

		animator.SetTrigger("Pickup");
	}

	public void ToolToHand(Tool.ToolType toolType)
	{
		Hand? h = GetHandByToolType(toolType);
		if (h != null && holster[h.Value] )
		{
			DrawTool(h.Value);
		}
	}

	public void DrawTool(Hand h)
	{
		Tool t = holster[h];
		if (t == null)
		{
			if (hands[h]?.GetComponent<Tool>() != null)
				t = (Tool)hands[h];
			else
				return;
		}

		if (t == null)
			return;

		/*if (((h == Hand.Left && !leftHandFree) || (h == Hand.Right && !rightHandFree)) && t.toolstatus != Tool.ToolStatus.InHand)
			return;*/

		if (holster[h] != null && t.toolstatus == Tool.ToolStatus.InHand)
			return;

		if ((hands[h] != null) && t.toolstatus == Tool.ToolStatus.Holstered)
			return;

		animator.SetBool("Hide", (t.toolstatus == Tool.ToolStatus.InHand));
		animator.SetBool("RightHand", (h == Hand.Right));
		animator.SetTrigger("DrawTrigger");
		float animTime = animator.GetCurrentAnimatorStateInfo(1).length;
		GameManager.instance.ExecuteAction(() => {
			if (h == Hand.Left)
			{
				if (t.toolstatus == Tool.ToolStatus.Holstered)
				{
					t.transform.SetParent(leftHandBone);
					t.transform.localPosition = leftinHandOffsetPosition;
					t.transform.localRotation = Quaternion.Euler(leftinHandOffsetRotation);
					t.toolstatus = Tool.ToolStatus.InHand;
					holster[h] = null;
					hands[h] = t;
				}
				else
				{
					t.transform.SetParent(hipsBone);
					t.transform.localPosition = rightholsteredOffsetPosition;
					t.transform.localRotation = Quaternion.Euler(rightholsteredOffsetRotation);
					t.toolstatus = Tool.ToolStatus.Holstered;
					holster[h] = t;
					hands[h] = null;
				}
			}
			else
			{
				if (t.toolstatus == Tool.ToolStatus.Holstered)
				{
					t.transform.SetParent(rightHandBone);
					t.transform.localPosition = rightinHandOffsetPosition;
					t.transform.localRotation = Quaternion.Euler(rightinHandOffsetRotation);
					t.toolstatus = Tool.ToolStatus.InHand;
					hands[h] = t;
					holster[h] = null;
				}
				else
				{
					t.transform.SetParent(hipsBone);
					t.transform.localPosition = leftholsteredOffsetPosition;
					t.transform.localRotation = Quaternion.Euler(leftholsteredOffsetRotation);
					t.toolstatus = Tool.ToolStatus.Holstered;
					holster[h] = t;
					hands[h] = null;
				}
			}
		}, (t.toolstatus == Tool.ToolStatus.InHand) ? animTime*0.8f : animTime*0.3f);
	}

	public Hand? GetHolsterByToolType(Tool.ToolType t)
	{
		if (holster[Hand.Left] != null && holster[Hand.Left].toolType == t)
			return Hand.Left;

		if (holster[Hand.Right] != null && holster[Hand.Right].toolType == t)
			return Hand.Right;

		return null;
	}
	public Hand? GetHandByToolType(Tool.ToolType t)
	{
		if (hands[Hand.Left] != null && hands[Hand.Left] is Tool && ((Tool)hands[Hand.Left]).toolType == t)
			return Hand.Left;

		if (hands[Hand.Right] != null && hands[Hand.Right] is Tool && ((Tool)hands[Hand.Right]).toolType == t)
			return Hand.Right;

		return null;
	}

	public Tool GetToolByType(Tool.ToolType type)
	{
		if (holster[Hand.Left]?.toolType == type)
			return holster[Hand.Left];

		if (holster[Hand.Right]?.toolType == type)
			return holster[Hand.Right];

		if (hands[Hand.Left]?.GetComponent<Tool>() != null && ((Tool)hands[Hand.Left])?.toolType == type)
			return (Tool)hands[Hand.Left];

		if (hands[Hand.Right]?.GetComponent<Tool>() != null && ((Tool)hands[Hand.Right])?.toolType == type)
			return (Tool)hands[Hand.Right];

		return null;
	}

	public bool HasToolInHand(Tool.ToolType type)
	{
		if (hands[Hand.Left]?.GetComponent<Tool>() != null && ((Tool)hands[Hand.Left])?.toolType == type)
			return true;

		if (hands[Hand.Right]?.GetComponent<Tool>() != null && ((Tool)hands[Hand.Right])?.toolType == type)
			return true;

		return false;
	}

	public Hand? GetFreeHand()
	{
		if (hands[Hand.Left] == null)
			return Hand.Left;
		if (hands[Hand.Right] == null)
			return Hand.Right;

		return null;
	}

	public bool HasFreeHand(Hand h)
	{
		switch (h)
		{
			case Hand.Left:
				return (hands[h] == null);
			case Hand.Right:
				return (hands[h] == null);
			case Hand.Any:
				return (hands[Hand.Left] == null) || (hands[Hand.Right] == null);
			case Hand.Both:
				return (hands[Hand.Left] == null) && (hands[Hand.Right] == null);
		}
		return false;
	}

	public void Attack(IAttackable target)
	{
		if (target == null)
			return;

		Hand? h = GetHandByToolType(Tool.ToolType.Sword);
		if (h != null)
		{
			Sword s = ((Sword)hands[h.Value]);
			if (s.canAttack)
			{
				s.canAttack = false;
				animator.SetBool("RightHand", (h == Hand.Left) ? false : true);
				animator.SetTrigger("AttackTrigger");
				AudioSource.PlayClipAtPoint(GameManager.instance.wooshSwordSound, transform.position);
				GameManager.instance.ExecuteAction(() =>
				{
					target.ReceiveDamage(s.damage);

				}, 1);

				GameManager.instance.ExecuteAction(new Action(() => {
					s.canAttack = true;
				}), s.cooldown);
			}
		}
	}

	public void ClearOnTargetReachedListeners()
	{
		OnTargetReachedHandlers.Clear();
	}

	public void MoveItem(Item i, int index)
	{
		switch (index)
		{
			case -4:
				if (i != null)
				{
					i.transform.SetParent(hipsBone);
					i.transform.localPosition = rightholsteredOffsetPosition;
					i.transform.localRotation = Quaternion.Euler(rightholsteredOffsetRotation);
					((Tool)i).toolstatus = Tool.ToolStatus.Holstered;
				}
				PlayerController.instance.holster[PlayerController.Hand.Left] = (Tool)i;
				break;
			case -3:
				if (i != null)
				{
					i.transform.SetParent(leftHandBone);
					if (i is Tool t)
					{
						t.transform.localPosition = leftinHandOffsetPosition;
						t.transform.localRotation = Quaternion.Euler(leftinHandOffsetRotation);
						t.toolstatus = Tool.ToolStatus.Holstered;
					}
				}

				PlayerController.instance.hands[PlayerController.Hand.Left] = i;
				break;
			case -2:
				if (i != null)
				{
					i.transform.SetParent(hipsBone);
					if (i is Tool)
					{
						i.transform.localPosition = leftholsteredOffsetPosition;
						i.transform.localRotation = Quaternion.Euler(leftholsteredOffsetRotation);
						((Tool)i).toolstatus = Tool.ToolStatus.Holstered;
					}
				}
				PlayerController.instance.holster[PlayerController.Hand.Right] = (Tool)i;
				break;
			case -1:
				if (i != null)
				{
					i.transform.SetParent(rightHandBone);
					if (i is Tool tt)
					{
						tt.transform.localPosition = rightinHandOffsetPosition;
						tt.transform.localRotation = Quaternion.Euler(rightinHandOffsetRotation);
						tt.toolstatus = Tool.ToolStatus.InHand;
					}
				}
				PlayerController.instance.hands[PlayerController.Hand.Right] = i;
				break;
			default:
				PlayerController.instance.items[index] = i;
				break;
		}

	}

	public bool PutItemToHands(Item i, Hand h=Hand.Any)
	{
		if (i.handlingMethod == Item.HandlingMethod.InOneHand)
		{
			Hand? freehand = null;
			if (h == Hand.Any)
				freehand = GetFreeHand();
			else
			{
				if (h != Hand.Both && HasFreeHand(h))
					freehand = h;
			}

			if (freehand == null)
				return false;

			hands[freehand.Value] = i;
			i.gameObject.SetActive(true);
			i.transform.SetParent((freehand == Hand.Left) ? leftHandBone : rightHandBone);
			i.transform.localPosition = Vector3.zero;
			i.transform.localRotation = Quaternion.identity;

			return true;
		}
		else
		{
			if (HasFreeHand(Hand.Both))
			{
				hands[Hand.Left] = hands[Hand.Right] = i;
				i.gameObject.SetActive(true);
				i.transform.SetParent((i.handlingMethod == Item.HandlingMethod.InTwoHands) ? bothHandsPosition : rightHandBone);
				i.transform.localPosition = Vector3.zero;
				i.transform.localRotation = Quaternion.identity;
				
				return true;
			}
			return false;
		}
	}

	public bool HasResource(Resource.ResourceType t, int count=1)
	{
		int c = 0;
		if (hands[Hand.Left]?.GetComponent<Resource>() != null &&
			hands[Hand.Left].GetComponent<Resource>().resourceType == t)
			c++;

		if (hands[Hand.Right]?.GetComponent<Resource>() != null &&
			hands[Hand.Right].GetComponent<Resource>().resourceType == t)
			c++;

		List<Item> i = items.FindAll(x => x?.GetComponent<Resource>() != null && x.GetComponent<Resource>().resourceType == t);
		c += i.Count;

		if (c >= count)
			return true;

		return false;
	}

	public bool DrainEnergy(float en)
	{
		if (energy > 20 && energy-en <= 20)
			GameUI.instance.ShowHint("Zaczynasz być zmęczony, rozważ powrót do wioski by przespać się przy ognisku");

		if (energy >= en)
		{
			energy -= en;
			if (energy <= 0)
				energy = 0;

			return true;
		}
		GameUI.instance.ShowHint("Nie masz wystarczająco energii by to zrobić.\nWróć do wioski i prześpij się przy ognisku");
		return false;
	}


	private void OnDrawGizmos()
	{
		//Gizmos.DrawCube(moveTargetLocation, Vector3.one);
	}
}