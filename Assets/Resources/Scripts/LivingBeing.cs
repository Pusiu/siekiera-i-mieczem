using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class LivingBeing : MonoBehaviour, IInteractable, IAttackable
{
	public int health = 5;

	public NavMeshAgent agent;
	public Animator animator;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
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
