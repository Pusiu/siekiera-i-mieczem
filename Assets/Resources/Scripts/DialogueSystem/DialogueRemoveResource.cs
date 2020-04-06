using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueRemoveResource : DialogueAction
{
	public Resource.ResourceType t;
	public int count = 1;

	public override bool Execute(NPC npc)
	{
		int c = 0;
		if (PlayerController.instance.hands[PlayerController.Hand.Left]?.GetComponent<Resource>()?.resourceType == t)
		{
			Destroy(PlayerController.instance.hands[PlayerController.Hand.Left].gameObject);
			PlayerController.instance.hands[PlayerController.Hand.Left] = null;
			c++;
		}
		if (c >= count)
		{
			PlayerController.instance.animator.SetTrigger("Pickup");
			PlayerController.instance.RefreshHandAnimation();
			return true;
		}

		if (PlayerController.instance.hands[PlayerController.Hand.Right]?.GetComponent<Resource>()?.resourceType == t)
		{
			Destroy(PlayerController.instance.hands[PlayerController.Hand.Right].gameObject);
			PlayerController.instance.hands[PlayerController.Hand.Right] = null;
			c++;
		}
		if (c >= count)
		{
			PlayerController.instance.animator.SetTrigger("Pickup");
			PlayerController.instance.RefreshHandAnimation();
			return true;
		}

		for (int i=0; i < PlayerController.instance.items.Count;i++)
		{
			if (c >= count)
				return true;

			Item it = PlayerController.instance.items[i];

			if (it != null)
			{
				if (it.GetComponent<Resource>()?.resourceType == t)
				{
					Destroy(PlayerController.instance.items[i].gameObject);
					PlayerController.instance.items[i] = null;
					c++;
				}
			}
		}
		PlayerController.instance.RefreshHandAnimation();

		return true;
	}

	public override void DrawInspectorLine()
	{
		base.DrawInspectorLine();
		EditorGUILayout.LabelField("Removes x resources from player's inventory");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Resource type:");
		t = (Resource.ResourceType)EditorGUILayout.EnumPopup(t);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Count:");
		count = EditorGUILayout.IntField(count);
		EditorGUILayout.EndHorizontal();

	}
}
