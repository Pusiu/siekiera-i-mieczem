using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public List<BaseQuest> questList = new List<BaseQuest>();

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		if (GameUI.instance == null)
		{
			SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
			SceneManager.sceneLoaded += (s,e) =>
			{
				GetComponent<TimeAndWeather>().StartCycle();
			};
		}
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
