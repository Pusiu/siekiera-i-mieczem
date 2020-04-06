using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Collider))]
public class Animal : LivingBeing
{
	public float areaDistance = 30;

	Vector3 startPos;
	Vector3 targetPos;
	float timeout = 3;
	float lastTime;


	// Start is called before the first frame update
	void Start()
    {
		startPos = transform.position;
		lastTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.hasPath || agent.velocity.sqrMagnitude < 1)
		{
			if (Time.time > lastTime + timeout)
			{
				lastTime = Time.time;
				agent.SetDestination(startPos + new Vector3(Random.Range(-areaDistance, areaDistance), 0, Random.Range(-areaDistance, areaDistance)));
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
