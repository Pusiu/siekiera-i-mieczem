using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public AudioClip wooshSound;
	public AudioClip wooshSwordSound;
	public AudioClip buildingSound;
	public bool autoEndgame = true;

	public List<BaseQuest> questList = new List<BaseQuest>();
	public List<LivingBeing> npcs;
	public List<MapBoundCollider> mapBounds;

	bool gameOver = false;
	// Start is called before the first frame update

	void Start()
	{
		instance = this;
		if (GameUI.instance == null)
		{
			SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
			SceneManager.sceneLoaded += (s, e) =>
			{
				GetComponent<TimeAndWeather>().StartCycle();
				GameUI.instance.SetupQuestLogEntries();
			};
		}

		mapBounds.AddRange(FindObjectsOfType<MapBoundCollider>());

		/*Scene sc = SceneManager.GetActiveScene();
		npcs.AddRange(SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<NPC>()));*/
		questList.Clear();
		questList.AddRange(FindObjectsOfType<BaseQuest>());
		InvokeRepeating("CheckQuestStatuses", 1, 1);
	}

	public void CheckIfInBounds()
	{
		foreach (MapBoundCollider m in mapBounds)
		{
			if (m.GetComponent<Collider>().bounds.Contains(PlayerController.instance.movePoint.transform.position))
			{
				MapBoundCollider.StopPlayer();
				return;
			}
		}
	}

	public void CheckQuestStatuses()
	{
		if (gameOver || !autoEndgame)
			return;

		if (questList.FindAll(x => x.questState != BaseQuest.QuestState.Completed && x.isMainQuest).Count == 0)
		{
			gameOver = true;
			ExecuteAction(() =>
			{
				GameUI.instance.ShowGameWinScreen();
			}, 5);
		}

	}

	public void RebuildNavMesh()
	{
	}

    // Update is called once per frame
    void Update()
    {
		CheckActionQueue();
    }



	void CheckActionQueue()
	{
		for (int i=0; i < actionQueue.Count;i++)
		{
			Tuple<Action, float> a = actionQueue[i];
			if (Time.time >= a.Item2)
			{
				a.Item1.Invoke();
				actionQueue.RemoveAt(i);
			}
		}
	}


	public List<Tuple<Action, float>> actionQueue = new List<Tuple<Action, float>>();
	public void ExecuteAction(Action action, float timeout)
	{
		actionQueue.Add(new Tuple<Action, float>(action, Time.time+timeout));
	}
}
