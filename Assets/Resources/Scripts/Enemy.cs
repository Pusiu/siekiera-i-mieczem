using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingBeing
{
	public BoxCollider activityZone;
	public int lastMoveTime;
	public bool isInCombat = false;
	public float timeBetweenAttacks = 2;
	public float attackDistance = 2;
	public int damage = 20;
	public float spottingDistance = 30;
	public bool canAttack = true;

	float speed;
	private void Start()
	{
		attackDistance = agent.stoppingDistance + 1;
		speed = agent.speed;

		if (id == -1)
		{
			Debug.LogError("Enemy has no id!");
		}
	}

	private void Update()
	{
		if (health <= 0)
			return;

		if (isInCombat)
		{
			if (PlayerController.instance.health <=0)
			{
				isInCombat = false;
				return;
			}

			agent.speed = speed;
			if (!activityZone.bounds.Contains(transform.position))
			{
				MoveTo(transform.position);
				OnPlayerLeaveArea();
			}
			if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) <= attackDistance)
			{
				if (canAttack)
					Attack();
			}
			else
			{
				MoveTo(PlayerController.instance.transform.position);
			}
		}
		else
		{
			agent.speed = speed/2;
			if (agent.velocity.magnitude <= 0.1f)
			{
				Vector3 randomPoint = activityZone.transform.position + new Vector3(Random.Range(-1, 1) * activityZone.size.x / 2, 0, Random.Range(-1, 1) * activityZone.size.z / 2);
				MoveTo(randomPoint);
			}

			if (activityZone.bounds.Contains(PlayerController.instance.transform.position) && PlayerController.instance.health > 0)
			{
				if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) <= spottingDistance)
				{
					Vector3 dir = PlayerController.instance.transform.position - transform.position;
					if (Mathf.Abs(Vector3.Dot(dir, transform.right)) >= 0.5f)
					{
						OnPlayerSpotted();
					}
				}
			}
		}

		animator.SetFloat("Velocity", agent.velocity.magnitude / speed);
	}

	public void OnPlayerSpotted()
	{
		isInCombat = true;
		string text = "Będziesz żałował że tutaj przyszedłeś";
		GameUI.instance.currentSpeechFocus = gameObject;
		GameUI.instance.OnTypewriteEnded += OnTypewriteEnded;
		GetComponent<AudioSource>().Play();
		GameUI.instance.Typewrite(text);
	}

	private void OnTypewriteEnded()
	{
		GetComponent<AudioSource>().Stop();
		GameUI.instance.speechBubble.gameObject.SetActive(false);
	}

	public void OnPlayerLeaveArea()
	{
		isInCombat = false;
		string text = "Jeszcze cię dorwę!";
		GameUI.instance.currentSpeechFocus = gameObject;

		GameUI.instance.OnTypewriteEnded += OnTypewriteEnded;
		GetComponent<AudioSource>().Play();
		GameUI.instance.Typewrite(text);
	}

	void MoveTo(Vector3 target)
	{
		if (agent.enabled)
			agent.SetDestination(target);
	}

	public override void ReceiveDamage(int damage)
	{
		if (!isInCombat && health > 0)
			isInCombat = true;

		base.ReceiveDamage(damage);
		if (health <=0)
		{
			agent.SetDestination(transform.position);
			animator.SetTrigger("Die");
		}
	}

	public void Attack()
	{
		animator.SetBool("RightHand", true);
		animator.SetTrigger("AttackTrigger");
		canAttack = false;
		GameManager.instance.ExecuteAction(() =>
		{
			PlayerController.instance.ReceiveDamage(damage);
		}, timeBetweenAttacks/2);

		GameManager.instance.ExecuteAction(() =>
		{
			canAttack = true;

		}, timeBetweenAttacks);
	}
}
