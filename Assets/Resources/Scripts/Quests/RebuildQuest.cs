using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebuildQuest : BaseQuest
{
	public int woodAmount;
	public int stoneAmount;
	public ResourceArea resourceArea;
	public GameObject originalBuilding;
	public GameObject rebuildedBuildingPrefab;

	// Start is called before the first frame update
	void Start()
	{
		resourceArea.OnResourceAreaUpdateEvent += ResourceArea_OnResourceAreaUpdateEvent;
	}

	private void ResourceArea_OnResourceAreaUpdateEvent()
	{
		//might potentialy delete more objects than needed
		if (resourceArea.resourcesCount[Resource.ResourceType.Stone] >= stoneAmount)
		{
			if (resourceArea.resourcesCount[Resource.ResourceType.Wood] >= woodAmount)
			{
				SetState(QuestState.Completed);
				GameUI.instance.FadeInRebuildingScreen();
				PlayerController.instance.canMove = false;
				GameManager.instance.ExecuteAction(() =>
				{
					GameUI.instance.FadeOutRebuildingScreen();
					PlayerController.instance.canMove = true;
					resourceArea.RemoveResources();
				}, 2.0f);
			}
		}
	}


	// Update is called once per frame
	void Update()
    {
        
    }

	public override string GetDescription()
	{
		return $"Przynieś następującą ilość zasobów:\n" +
				$"Kamień:{resourceArea.resourcesCount[Resource.ResourceType.Stone]}/{stoneAmount}\n" +
				$"Drewno:{resourceArea.resourcesCount[Resource.ResourceType.Wood]}/{woodAmount}";
	}
}
