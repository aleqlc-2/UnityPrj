using UnityEngine;

public class IdleState : State<EnemyController>
{
	EnemyController enemy;

	// Entered Idle State
	public override void Enter(EnemyController owner)
	{
		enemy = owner;

		enemy.Animator.SetBool("combatMode", false);
	}

	// Executing Idle State
	public override void Execute()
	{
		enemy.Target = enemy.FindTarget();
		if (enemy.Target != null)
		{
			enemy.AlertNearbyEnemies();
			enemy.ChangeState(EnemyStates.CombatMovement);
		}
	}

	// Exiting Idle State
	public override void Exit()
	{

	}
}
