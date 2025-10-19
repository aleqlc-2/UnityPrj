using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float lifeTimeAmount = 0.8f;
	[SerializeField] private LayerMask groundLayerMask;
	private Collider2D coll;

	private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
	[SerializeField] private LayerMask playerLayerMask;

	[Networked] private TickTimer lifeTimeTimer { get; set; }
	[Networked] private NetworkBool didHitSomething { get; set; }
	[SerializeField] private int bulletDmg = 10;

	public override void Spawned()
	{
		coll = GetComponent<Collider2D>();
		lifeTimeTimer = TickTimer.CreateFromSeconds(Runner, lifeTimeAmount);
	}

	public override void FixedUpdateNetwork()
	{
		if (!didHitSomething)
		{
			CheckIfHitGround();
			CheckIfWeHitPlayer();
		}
		
		if (lifeTimeTimer.ExpiredOrNotRunning(Runner) == false && !didHitSomething)
		{
			transform.Translate(transform.right * moveSpeed * Runner.DeltaTime, Space.World);
		}

		if (lifeTimeTimer.Expired(Runner) || didHitSomething)
		{
			lifeTimeTimer = TickTimer.None;
			Runner.Despawn(Object);
		}
	}

	private void CheckIfHitGround()
	{
		var groundCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position, coll.bounds.size, 0, groundLayerMask);
		if (groundCollider != default) // groundCollider != null
		{
			didHitSomething = true;
		}
	}

	private void CheckIfWeHitPlayer()
	{
		Runner.LagCompensation.OverlapBox(transform.position, coll.bounds.size, Quaternion.identity, Object.InputAuthority, hits, playerLayerMask);

		if (hits.Count > 0)
		{
			foreach (var item in hits)
			{
				if (item.Hitbox != null)
				{
					var player = item.Hitbox.GetComponentInParent<PlayerController>();
					var didNotHitOurOwnPlayer = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId; // 총을 쏜 플레이어와는 충돌감지X

					if (didNotHitOurOwnPlayer && player.PlayerIsAlive)
					{
						if (Runner.IsServer) // rpc는 서버만 실행
						{
							player.GetComponent<PlayerHealthController>().Rpc_ReducePlayerHealth(bulletDmg);
						}

						didHitSomething = true;
						break;
					}
				}
			}
		}
	}
}
