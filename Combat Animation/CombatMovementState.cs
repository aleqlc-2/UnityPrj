using UnityEngine;

public enum AICombatStates { Idle, Chase, Circling }

public class CombatMovementState : State<EnemyController>
{
	[SerializeField] private float distanceToStand = 3f;
	[SerializeField] private float circlingSpeed = 20f;
	[SerializeField] private float adjustDistanceThreshold = 1f;
	[SerializeField] private Vector2 idleTimeRange = new Vector2(2, 5);
	[SerializeField] private Vector2 circlingTimeRange = new Vector2(3, 6);
	float timer = 0f;
	int circlingDir = 1;

	private EnemyController enemy;

	AICombatStates state;

	// Entered Chase State
	public override void Enter(EnemyController owner)
	{
		enemy = owner;
		enemy.NavAgent.stoppingDistance = distanceToStand;
		enemy.CombatMovementTimer = 0f;
		enemy.Animator.SetBool("combatMode", true);
	}

	// Executing Chase State
	public override void Execute()
	{
		if (enemy.Target != null)
		{
			enemy.Target = enemy.FindTarget();
			if (enemy.Target == null)
			{
				enemy.ChangeState(EnemyStates.Idle);
				return;
			}
		}

		if (enemy.Target.Health <= 0)
		{
			enemy.Target = null;
			enemy.ChangeState(EnemyStates.Idle);
			return;
		}

		if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) > distanceToStand + adjustDistanceThreshold)
		{
			StartChase();
		}

		if (state == AICombatStates.Idle)
		{
			if (timer <= 0)
			{
				if(Random.Range(0, 2) == 0)
				{
					StartIdle();
				}
				else
				{
					StartCircling();
				}
			}
		}
		else if (state == AICombatStates.Chase)
		{
			if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= distanceToStand + 0.03f)
			{
				StartIdle();
				return;
			}

			enemy.NavAgent.SetDestination(enemy.Target.transform.position);
		}
		else if (state == AICombatStates.Circling)
		{
			if (timer <= 0)
			{
				StartIdle();
				return;
			}

			var vecToTarget = enemy.transform.position - enemy.Target.transform.position;
			var rotatedPos = Quaternion.Euler(0, circlingSpeed * circlingDir * Time.deltaTime, 0) * vecToTarget;
			enemy.NavAgent.Move(rotatedPos - vecToTarget);
			enemy.transform.rotation = Quaternion.LookRotation(-rotatedPos);
		}
		
		if (timer > 0)
		{
			timer -= Time.deltaTime;
		}

		enemy.CombatMovementTimer += Time.deltaTime;
	}

	void StartChase()
	{
		state = AICombatStates.Chase;
	}
	private void StartIdle()
	{
		state = AICombatStates.Idle;
		timer = Random.Range(idleTimeRange.x, idleTimeRange.y);
	}

	private void StartCircling()
	{
		state = AICombatStates.Circling;
		enemy.NavAgent.ResetPath();
		timer = Random.Range(circlingTimeRange.x, circlingTimeRange.y);
		circlingDir = Random.Range(0, 2) == 0 ? 1 : -1;
	}

	// Exiting Chase State
	public override void Exit()
	{
		enemy.CombatMovementTimer = 0f;
	}
}
