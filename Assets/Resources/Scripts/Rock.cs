using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : ResourceGenerator
{
	public GameObject resourcePrefab;
	public int hitsToDestroy = 3;

	public AudioClip onDestroyedSound;
	public AudioClip hitSound;

	public override bool Gather()
	{
		if (base.Gather())
		{
			Mine();
			return true;
		}
		return false;
	}

	public void Mine()
	{
		Tool t = PlayerController.instance.GetToolByType(Tool.ToolType.Pickaxe);
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
				if (hitSound != null)
					AudioSource.PlayClipAtPoint(hitSound, transform.position);

				PlayerController.instance.transform.LookAtYOnly(transform.position);
				if (hitsToDestroy <=0)
				{
					if (onDestroyedSound != null)
						AudioSource.PlayClipAtPoint(onDestroyedSound, transform.position);

					Vector3 c = GetComponent<Renderer>().bounds.center;
					Vector3 s = GetComponent<Renderer>().bounds.extents;
					for (int i = 0; i < count; i++)
					{
						GameObject go = Instantiate(resourcePrefab);
						go.transform.position = c;
						go.transform.position += new Vector3(s.x * Random.Range(-0.25f, 1), s.y * Random.Range(-.25f, 1), s.z * Random.Range(-0.25f, 1));
						if (go.GetComponent<Rigidbody>()==null)
							go.AddComponent<Rigidbody>();
					}
					Destroy(gameObject);
					return;
				}
				Invoke("Mine", 2);
			}
		}
		else
		{
			if (canBePickedUp)
				Pickup();
		}
	}
}
