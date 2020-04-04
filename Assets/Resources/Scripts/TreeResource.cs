using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : ResourceGenerator
{
	public GameObject logPrefab;
	public int hitsToDestroy = 3;

	public override void Gather()
	{
		base.Gather();
		Chop();
	}

	public void Chop()
	{
		Tool t = PlayerController.instance.GetToolByType(Tool.ToolType.Axe);
		if (t != null)
		{
			if (PlayerController.instance.interactionTarget == gameObject)
			{
				PlayerController.instance.animator.SetBool("LeftHand", (PlayerController.instance.hands[PlayerController.Hand.Left] == t) ? true : false);
				PlayerController.instance.animator.SetBool("RightHand", (PlayerController.instance.hands[PlayerController.Hand.Right] == t) ? true : false);
				PlayerController.instance.animator.SetTrigger("AttackTrigger");
				hitsToDestroy--;
				PlayerController.instance.transform.LookAtYOnly(transform.position);
				if (hitsToDestroy == 0)
				{
						if (GetComponent<Rigidbody>() == null)
							gameObject.AddComponent<Rigidbody>();
					
					Vector3 c = GetComponentInChildren<Renderer>().bounds.center;
					Vector3 s = GetComponentInChildren<Renderer>().bounds.extents;
					gameObject.GetComponent<Rigidbody>().AddForceAtPosition(PlayerController.instance.transform.forward*5, c + new Vector3(0, s.y, 0), ForceMode.Force);

					GameManager.instance.ExecuteAction(() =>
					{
						c = GetComponentInChildren<Renderer>().bounds.center;
						s = GetComponentInChildren<Renderer>().bounds.extents;
						GameObject go = Instantiate(logPrefab);
						go.transform.position = c;
						go.transform.LookAt(transform.position + transform.up * 5);
						go.AddComponent<Rigidbody>();
						Destroy(gameObject);
					}, 4);

					return;
				}
				Invoke("Chop", 2);
			}
		}
	}
}
