using System.Collections;
using UnityEngine;

public class GettingHitState : State<EnemyController>
{
    private EnemyController enemy;
	[SerializeField] private float stunTime = 0.5f;

	public override void Enter(EnemyController owner)
	{
		StopAllCoroutines();

		enemy = owner;
		enemy.Fighter.OnHitComplete += () => StartCoroutine(GoToCombatMovement());
	}

	private IEnumerator GoToCombatMovement()
	{
		yield return new WaitForSeconds(stunTime);

		if (!enemy.IsInState(EnemyStates.Dead))
			enemy.ChangeState(EnemyStates.CombatMovement);
	}
}
