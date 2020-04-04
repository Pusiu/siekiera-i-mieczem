using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogResource : ResourceGenerator
{
	public int hitsToDestroy = 3;
	public GameObject branchPrefab;

	public override bool Gather()
	{
		if (base.Gather())
		{
			Chop();
			return true;
		}
		return false;
	}

	public void Chop()
	{
		Tool t = PlayerController.instance.GetToolByType(requiredToolType);
		if (t != null)
		{
			if (PlayerController.instance.interactionTarget == gameObject)
			{
				if (!PlayerController.instance.DrainEnergy(workEnergyDrainPerHit))
					return;

				PlayerController.instance.animator.SetBool("LeftHand", (PlayerController.instance.hands[PlayerController.Hand.Left] == t) ? true : false);
				PlayerController.instance.animator.SetBool("RightHand", (PlayerController.instance.hands[PlayerController.Hand.Right] == t) ? true : false);
				PlayerController.instance.animator.SetTrigger("AttackTrigger");
				hitsToDestroy--;
				PlayerController.instance.transform.LookAtYOnly(transform.position);
				if (hitsToDestroy <= 0)
				{
					Vector3 c = GetComponent<Renderer>().bounds.center;
					Vector3 s = GetComponent<Renderer>().bounds.extents;
					for (int i = 0; i < count; i++)
					{
						GameObject go = Instantiate(branchPrefab);
						go.transform.position = c;
						go.transform.position += new Vector3(s.x * Random.Range(-0.25f, 1), s.y * Random.Range(-.25f, 1), s.z * Random.Range(-0.25f, 1));
						if (go.GetComponent<Rigidbody>() == null)
							go.AddComponent<Rigidbody>();
					}
					Destroy(gameObject);
					return;
				}
			}
		}
	}
}
