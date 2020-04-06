using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public static GameUI instance;
	public Image timeDial;
	public Slider hpSlider;
	public Slider energySlider;
	public RectTransform speechBubble;
	public GameObject gameOverScreen;
	public GameObject gameWinScreen;
	public GameObject menu;
	public GameObject hintPanel;
	public GameObject guideScreen;
	public GameObject guideScreenEntriesTitles;
	public GameObject guideScreenEntriesContents;
	public GameObject timePassOverlay;
	public GameObject compass;

	public GameObject questLogContent;
	public GameObject questLogEntryPrefab;
	public Dictionary<string, GameObject> questLogEntries = new Dictionary<string, GameObject>();

	public GameObject currentSpeechFocus = null;
	public float typewriteLetterTime = 0.1f;

	public GameObject inventoryScreen;
	public GameObject itemInfo;
	public Image preview;
	public GameObject inventorySlotPrefab;
	public GameObject inventoryGrid;
	public List<GameObject> inventorySlots = new List<GameObject>();
	public Color emptySlotColor;
	public Sprite defaultImage;

	public int lastDraggedIndex = 0;
	public Image dropSlot;
	public Image rightHandSlot;
	public Image rightHolsterSlot;
	public Image leftHandSlot;
	public Image leftHolsterSlot;

	private void Awake()
	{
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		EventTrigger.Entry eventDrag = new EventTrigger.Entry();
		eventDrag.eventID = EventTriggerType.Drag;
		eventDrag.callback.AddListener((eventData) => { InventorySlotEventHandler_OnDrag(eventData); });

		EventTrigger.Entry eventEndDrag= new EventTrigger.Entry();
		eventEndDrag.eventID = EventTriggerType.EndDrag;
		eventEndDrag.callback.AddListener((eventData) => { InventorySlotEventHandler_OnEndDrag(eventData); });

		EventTrigger.Entry eventDrop = new EventTrigger.Entry();
		eventDrop.eventID = EventTriggerType.Drop;
		eventDrop.callback.AddListener((eventData) => { InventorySlotEventHandler_OnDrop(eventData); });

		EventTrigger.Entry eventEnter = new EventTrigger.Entry();
		eventEnter.eventID = EventTriggerType.PointerEnter;
		eventEnter.callback.AddListener((eventData) => { InventorySlotEventHandler_OnEnter(eventData); });

		EventTrigger.Entry eventExit = new EventTrigger.Entry();
		eventExit.eventID = EventTriggerType.PointerExit;
		eventExit.callback.AddListener((eventData) => { InventorySlotEventHandler_OnExit(eventData); });

		List<EventTrigger.Entry> allEvents = new List<EventTrigger.Entry>() { eventDrag, eventEndDrag, eventDrop,eventEnter,eventExit };

		rightHandSlot.color = emptySlotColor;
		rightHandSlot.transform.parent.GetComponent<EventTrigger>().triggers.AddRange(allEvents);
		rightHandSlot.name = "-1";

		rightHolsterSlot.color = emptySlotColor;
		rightHolsterSlot.transform.parent.GetComponent<EventTrigger>().triggers.AddRange(allEvents);
		rightHolsterSlot.name = "-2";

		leftHandSlot.color = emptySlotColor;
		leftHandSlot.transform.parent.GetComponent<EventTrigger>().triggers.AddRange(allEvents);
		leftHandSlot.name = "-3";

		leftHolsterSlot.color = emptySlotColor;
		leftHolsterSlot.transform.parent.GetComponent<EventTrigger>().triggers.AddRange(allEvents);
		leftHolsterSlot.name = "-4";

		dropSlot.name = "-5";
		dropSlot.transform.parent.GetComponent<EventTrigger>().triggers.Add(eventDrop);

		for (int i = 0; i < PlayerController.instance.inventoryCapacity; i++)
		{
			GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid.transform);
			slot.transform.GetChild(0).GetComponent<Image>().color = emptySlotColor;

			slot.GetComponent<EventTrigger>().triggers.AddRange(allEvents);
			slot.transform.GetChild(0).name = i.ToString();

			inventorySlots.Add(slot);
		}
		SetupGuideScreenCallbacks();
    }

	public void SetupQuestLogEntries()
	{
		foreach (BaseQuest q in GameManager.instance.questList)
		{
			GameObject log = Instantiate(questLogEntryPrefab, questLogContent.transform);
			log.SetActive(false);
			Text[] texts = log.GetComponentsInChildren<Text>();
			texts[0].text = q.questName;
			texts[1].text = "";
			questLogEntries.Add(q.name, log);
		}
	}

	void SetupGuideScreenCallbacks()
	{
		Dictionary<string,GameObject> contentList = new Dictionary<string,GameObject>();
		for (int i = 0; i < guideScreenEntriesContents.transform.childCount; i++)
		{
			GameObject g = guideScreenEntriesContents.transform.GetChild(i).gameObject;
			contentList.Add(g.name, g);
		}

		for (int i=0; i < guideScreenEntriesTitles.transform.childCount;i++)
		{
			GameObject en = guideScreenEntriesTitles.transform.GetChild(i).gameObject;
			GameObject con = contentList[en.name];

			if (con == null)
				Debug.LogWarning($"[GuideScreen]No matching content found for entry {en.name}");
			
			EventTrigger.Entry ev = new EventTrigger.Entry();
			ev.eventID = EventTriggerType.PointerClick;
			ev.callback.AddListener((eventData) => {
				SelectGuideScreenEntry(con);
			});
			en.GetComponent<EventTrigger>().triggers.Add(ev);
		}
	}

	Item GetItemByIndex(int index)
	{
		switch(index)
		{
			case -4:
				return PlayerController.instance.holster[PlayerController.Hand.Left];
			case -3:
				return PlayerController.instance.hands[PlayerController.Hand.Left];
			case -2:
				return PlayerController.instance.holster[PlayerController.Hand.Right];
			case -1:
				return PlayerController.instance.hands[PlayerController.Hand.Right];
			case int n when n >= 0:
				return PlayerController.instance.items[index];
			default:
				return null;
		}
	}

	void InventorySlotEventHandler_OnDrag(BaseEventData data)
	{
		lastDraggedIndex = Convert.ToInt32(data.selectedObject.transform.GetChild(0).gameObject.name);
		Item i = GetItemByIndex(lastDraggedIndex);
		if (i != null)
		{
			preview.gameObject.SetActive(true);
			preview.GetComponentInChildren<Image>().sprite = i.image;
		}
	}

	void InventorySlotEventHandler_OnEnter(BaseEventData data)
	{
		GameObject target = ((PointerEventData)data).pointerCurrentRaycast.gameObject;
		if (target == null || target.transform.childCount ==0)
			return;

		target = target.transform.GetChild(0)?.gameObject;

		int index = Convert.ToInt32(target.name);
		Item i = GetItemByIndex(index);
		if (i != null)
		{
			itemInfo.gameObject.SetActive(true);
			Text[] t = itemInfo.GetComponentsInChildren<Text>();
			t[0].text = i.itemName;
			t[1].text = i.GetItemDescription();
		}
	}
	void InventorySlotEventHandler_OnExit(BaseEventData data)
	{
		itemInfo.gameObject.SetActive(false);
	}

	void InventorySlotEventHandler_OnEndDrag(BaseEventData data)
	{
		preview.gameObject.SetActive(false);
	}

	void InventorySlotEventHandler_OnDrop(BaseEventData data)
	{
		PointerEventData d = new PointerEventData(EventSystem.current);
		d.position = Input.mousePosition;
		List<RaycastResult> r = new List<RaycastResult>();
		EventSystem.current.RaycastAll(d, r);

		Item s = GetItemByIndex(lastDraggedIndex);
		if (s == null)
			return;

		int destIndex = Convert.ToInt32(r[0].gameObject.name);
		Debug.Log($"From {lastDraggedIndex} to {destIndex}");

		Item dest = GetItemByIndex(destIndex);


		if (destIndex == lastDraggedIndex)
			return;

		if (destIndex == -5)
		{
			s.transform.SetParent(null);
			Vector3 up = Vector3.up * 2;
			if (s.transform.GetComponentInChildren<Collider>() != null)
			{
				up = new Vector3(0,s.transform.GetComponentInChildren<Collider>().bounds.extents.y,0);
			}

			s.transform.position = PlayerController.instance.transform.position + up + PlayerController.instance.transform.forward;
			s.transform.rotation = Quaternion.identity;

			/*if (PlayerController.instance.hands[PlayerController.Hand.Left] == s)
				PlayerController.instance.leftHandFree = true;*/

			s.GetComponent<PickableObject>().PutDown();

			s.gameObject.SetActive(true);
			PlayerController.instance.MoveItem(null,lastDraggedIndex);
			return;
		}

		if (s is Tool t)
		{
			if (destIndex >= 0) //don't allow storing tools in inventory
			{
				return;
			}
		}
		else
		{
			if (destIndex == -2 || destIndex == -4) //don't allow holstering items which aren't tools
				return;

			if (s.handlingMethod != Item.HandlingMethod.InOneHand) //don't allow manipulating objects which can't be holded in one hand
				return;

			s.gameObject.SetActive(false);

			if (destIndex == -1 || destIndex == -3)
				PlayerController.instance.PutItemToHands(s, (destIndex == -1) ? PlayerController.Hand.Right : PlayerController.Hand.Left);
		}

		PlayerController.instance.MoveItem(s, destIndex);
		PlayerController.instance.MoveItem(dest, lastDraggedIndex);

	}


	void ResetItemsSprites()
	{
		rightHandSlot.sprite = defaultImage;
		rightHandSlot.color = emptySlotColor;
		rightHolsterSlot.sprite = defaultImage;
		rightHolsterSlot.color = emptySlotColor;
		leftHandSlot.sprite = defaultImage;
		leftHandSlot.color = emptySlotColor;
		leftHolsterSlot.sprite = defaultImage;
		leftHolsterSlot.color = emptySlotColor;
	}

	// Update is called once per frame
	void Update()
	{

		hpSlider.value = PlayerController.instance.health;
		hpSlider.GetComponentInChildren<Text>().text = $"Zdrowie: {hpSlider.value}/100";
		energySlider.value = PlayerController.instance.energy;
		energySlider.GetComponentInChildren<Text>().text = $"Energia: {energySlider.value}/100";

		compass.transform.rotation = Quaternion.Euler(0, 0, PlayerController.instance.cameraAngle);

		if (currentSpeechFocus != null)
		{
			speechBubble.position = Camera.main.WorldToScreenPoint(currentSpeechFocus.transform.position + Vector3.up * Mathf.Clamp((PlayerController.instance.cameraZoom + 5), 3, 4));
		}

		if (preview.gameObject.activeInHierarchy)
		{
			preview.transform.position = Input.mousePosition + new Vector3(preview.rectTransform.rect.width / 3, -preview.rectTransform.rect.height / 3);
		}

		if (inventoryScreen.activeInHierarchy)
		{
			itemInfo.transform.position = Input.mousePosition;

			ResetItemsSprites();
			foreach (PlayerController.Hand h in PlayerController.instance.hands.Keys)
			{
				if (PlayerController.instance.hands[h] != null)
				{
					Image im = (h == PlayerController.Hand.Left) ? leftHandSlot : rightHandSlot;

					im.sprite = PlayerController.instance.hands[h].image;
					im.color = Color.white;
				}
			}
			foreach (PlayerController.Hand h in PlayerController.instance.holster.Keys)
			{
				if (PlayerController.instance.holster[h] != null)
				{
					Image im = (h == PlayerController.Hand.Left) ? leftHolsterSlot : rightHolsterSlot;

					im.sprite = PlayerController.instance.holster[h].image;
					im.color = Color.white;
				}
			}

			for (int i = 0; i < PlayerController.instance.items.Count; i++)
			{
				Image im = inventorySlots[i].transform.GetChild(0).GetComponentInChildren<Image>();
				if (PlayerController.instance.items[i] != null)
				{
					im.sprite = PlayerController.instance.items[i].image;
					im.color = Color.white;
				}
				else
				{
					im.sprite = defaultImage;
					im.color = emptySlotColor;
				}
			}
		}

		foreach (BaseQuest q in GameManager.instance.questList)
		{
			if (q.questState == BaseQuest.QuestState.Active)
			{
				questLogEntries[q.name].SetActive(true);
				GameManager.instance.ExecuteAction(() =>
				{
					questLogEntries[q.name].GetComponentsInChildren<Text>()[1].text = q.GetDescription();
				}, 0.5f);
			}
			else
			{
				questLogEntries[q.name].SetActive(false);
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(questLogContent.GetComponent<RectTransform>());
    }

	public void ShowGuideScreen()
	{
		PlayerController.instance.canMove = false;
		PlayerController.instance.MoveTo(PlayerController.instance.gameObject);
		HideAllGuideScreenContents();
		guideScreen.SetActive(true);
		//Canvas.ForceUpdateCanvases();

	}
	public void HideGuideScreen()
	{
		guideScreen.SetActive(false);
		PlayerController.instance.canMove = true;
	}
	public void HideAllGuideScreenContents()
	{
		for (int i = 0; i < guideScreenEntriesContents.transform.childCount;i++)
		{
			guideScreenEntriesContents.transform.GetChild(i).gameObject.SetActive(false);
		}
	}
	public void SelectGuideScreenEntry(GameObject content)
	{
		HideAllGuideScreenContents();
		content.SetActive(true);
		LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
		//Canvas.ForceUpdateCanvases();
	}

	public void ToggleInventory()
	{
		inventoryScreen.SetActive(!inventoryScreen.activeInHierarchy);
		itemInfo.SetActive(false);
	}

	public void FadeInRebuildingScreen()
	{
		timePassOverlay.GetComponent<Image>().color = Color.white * 0;
		timePassOverlay.SetActive(true);
		timePassOverlay.GetComponent<Animator>().SetBool("Fade",true);
	}
	public void FadeOutRebuildingScreen()
	{
		timePassOverlay.GetComponent<Animator>().SetBool("Fade", false);
		GameManager.instance.ExecuteAction(() =>
		{
			timePassOverlay.SetActive(false);
		}, timePassOverlay.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
	}

	public void Typewrite(string text)
	{
		StopAllCoroutines();
		speechBubble.gameObject.SetActive(true);
		StartCoroutine(TypewriteCoroutine(text));
	}

	public void ShowHint(string text)
	{
		hintPanel.GetComponentInChildren<Text>().text = text;
		if (hideTime > Time.time)
		{
			StopCoroutine(HintCoroutine());
		}
		else
			hintPanel.GetComponent<Animator>().SetTrigger("HintTrigger");

		hideTime = Time.time + 5;
		StartCoroutine(HintCoroutine());
	}

	public void ShowGameWinScreen()
	{
		PlayerController.instance.canMove = false;
		gameWinScreen.gameObject.SetActive(true);
	}

	public void ShowDeathScreen()
	{
		PlayerController.instance.canMove = false;
		gameOverScreen.gameObject.SetActive(true);
	}

	public void ShowMenu()
	{
		PlayerController.instance.MoveTo(PlayerController.instance.gameObject);
		PlayerController.instance.canMove = false;
		menu.SetActive(true);
	}
	public void HideMenu()
	{
		PlayerController.instance.canMove = true;
		menu.SetActive(false);
	}

	public void ExitToMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	float hideTime;
	IEnumerator HintCoroutine()
	{
		while(Time.time < hideTime)
		{
			yield return new WaitForEndOfFrame();
		}
		hintPanel.GetComponent<Animator>().SetTrigger("HintTrigger");
	}

	public delegate void TypewriteEnded();
	public event TypewriteEnded OnTypewriteEnded;

	IEnumerator TypewriteCoroutine(string text)
	{
		speechBubble.GetComponentInChildren<Text>().text = "";

		for (int i=0; i < text.Length; i++)
		{
			speechBubble.GetComponentInChildren<Text>().text += text[i];
			yield return new WaitForSeconds(typewriteLetterTime);
		}
		if (OnTypewriteEnded != null)
			OnTypewriteEnded();
	}
}
