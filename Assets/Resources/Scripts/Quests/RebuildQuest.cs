using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebuildQuest : BaseQuest
{
	public int woodAmount;
	public int rockAmount;
	public ResourceArea resourceArea;
	public GameObject destroyedBuilding;
	public GameObject rebuildedBuilding;

	public bool activateNextQuestOnCompletion = true;
	public BaseQuest nextQuest;

	// Start is called before the first frame update
	void Start()
	{
		resourceArea.OnResourceAreaUpdateEvent += ResourceArea_OnResourceAreaUpdateEvent;
	}

	public override void SetState(QuestState state)
	{
		base.SetState(state);
		if (state == QuestState.Active)
		{
			resourceArea.gameObject.SetActive(true);
			resourceArea.SetResourceText(woodAmount, rockAmount);
		}
		else
		{
			resourceArea.gameObject.SetActive(false);
		}
	}

	private void ResourceArea_OnResourceAreaUpdateEvent()
	{
		resourceArea.SetResourceText(woodAmount, rockAmount);
		//might potentialy delete more objects than needed
		if (resourceArea.resourcesCount[Resource.ResourceType.Rock] >= rockAmount)
		{
			if (resourceArea.resourcesCount[Resource.ResourceType.Wood] >= woodAmount)
			{
				SetState(QuestState.Completed);
				GameUI.instance.FadeInRebuildingScreen();
				PlayerController.instance.canMove = false;
				GameManager.instance.GetComponent<AudioSource>().clip = GameManager.instance.buildingSound;
				GameManager.instance.GetComponent<AudioSource>().Play();
				GameManager.instance.ExecuteAction(() =>
				{
					rebuildedBuilding.SetActive(true);
					destroyedBuilding.SetActive(false);
					GameUI.instance.FadeOutRebuildingScreen();
					PlayerController.instance.canMove = true;
					resourceArea.RemoveResources();
					resourceArea.gameObject.SetActive(false);
					if (activateNextQuestOnCompletion && nextQuest != null)
						nextQuest.SetState(QuestState.Active);

				}, 5.0f);
			}
		}
	}


	// Update is called once per frame
	void Update()
    {
        
    }

	public override string GetDescription()
	{
		string s = $"Przynieś następującą ilość zasobów:\n";
		if (rockAmount > 0)
			s += $"Skała:{resourceArea.resourcesCount[Resource.ResourceType.Rock]}/{rockAmount}\n";
		if (woodAmount > 0)
			s += $"Drewno:{resourceArea.resourcesCount[Resource.ResourceType.Wood]}/{woodAmount}";

		return s;
	}
}
