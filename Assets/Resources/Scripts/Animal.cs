using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Collider))]
public class Animal : LivingBeing
{
	Vector3 targetPos;
	float timeout = 3;
	float lastTime;


	// Start is called before the first frame update
	void Start()
    {
		lastTime = Time.time;
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

	public override void ReceiveDamage(int damage)
	{
		base.ReceiveDamage(damage);
		Destroy(gameObject);
	}
}
