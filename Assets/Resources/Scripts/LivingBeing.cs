using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class LivingBeing : MonoBehaviour, IInteractable, IAttackable
{

	public int id = -1;
	public int health = 5;

	public GameObject stepPrefab;
	public NavMeshAgent agent;
	public Animator animator;

	public virtual void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		if (stepPrefab != null)
			stepPrefab = Instantiate(stepPrefab, transform, false);
	}

	public void PlayStep()
	{
		if (stepPrefab != null)
			stepPrefab.GetComponent<StepSound>().Play();
	}

	public virtual void OnInteraction()
	{
		PlayerController.instance.MoveTo(gameObject);
		PlayerController.instance.ToolToHand(Tool.ToolType.Sword);
		PlayerController.instance.OnTargetReached += (tar) =>
		{
			PlayerController.instance.Attack(this);
		};
	}

	public virtual void ReceiveDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			PlayerController.instance.interactionTarget = null;
			//Destroy(gameObject);
		}
	}
}
