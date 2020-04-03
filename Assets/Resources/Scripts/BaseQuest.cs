﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseQuest : MonoBehaviour
{
	public int questId = 0;
	public string questName;
	public enum QuestState { Hidden, Active, Completed};
	public QuestState questState = QuestState.Hidden;

	public void SetState(QuestState state)
	{
		questState = state;
	}

	public virtual string GetDescription()
	{
		return "No description";
	}
}