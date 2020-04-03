using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Collider))]
public class Animal : MonoBehaviour, IInteractable, IAttackable
{
	public int health = 5;

	Vector3 targetPos;
	float timeout = 3;
	float lastTime;
	NavMeshAgent agent;
	Animator animator;


	// Start is called before the first frame update
	void Start()
    {
		lastTime = Time.time;
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.hasPath)
		{
			if (Time.time > lastTime + timeout)
			{
				lastTime = Time.time;
				agent.SetDestination(transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)));
			}
		}
		animator.SetFloat("Velocity", agent.velocity.magnitude / agent.speed);
	}

	public void OnInteraction()
	{
		PlayerController.instance.MoveTo(gameObject);
		PlayerController.instance.ToolToHand(Tool.ToolType.Sword);
		PlayerController.instance.OnTargetReached += (tar) =>
		{
			PlayerController.instance.Attack(this);
		};
	}

	public void ReceiveDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			PlayerController.instance.interactionTarget = null;
			Destroy(gameObject);
		}
	}
}
