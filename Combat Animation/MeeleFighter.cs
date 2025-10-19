using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum AttackStates { Idle, Windup, Impact, Cooldown }

public class MeeleFighter : MonoBehaviour
{
    [field: SerializeField] public float Health { get; private set; } = 25f;

	[SerializeField] private List<AttackData> attacks;
	[SerializeField] private List<AttackData> longRangeAttacks;
    [SerializeField] private float longRangeAttackThreshold = 1.5f;
	public List<AttackData> Attacks => attacks;

    [SerializeField] private GameObject sword;
    private BoxCollider swordCollider;
    public AttackStates AttackState { get; private set; }
    private SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

	private Animator animator;
	public bool InAction { get; private set; } = false;

    bool doCombo;
    int comboCount = 0;

    public bool IsCounterable => AttackState == AttackStates.Windup && comboCount == 0;
    public bool InCounter { get; set; } = false;

    [SerializeField] private float rotationSpeed = 500f;

	// using System;
	public event Action<MeeleFighter> OnGotHit;
    public event Action OnHitComplete;

    public bool IsTakingHit { get; private set; } = false;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		if (sword != null)
        {
            swordCollider = sword.GetComponent<BoxCollider>();
            leftHandCollider = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
            rightHandCollider = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
            leftFootCollider = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
            rightFootCollider = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();
            
            DisableAllHitboxes();
		}
	}

    private void EnableHitbox(AttackData attack)
    {
        switch (attack.HitboxToUse)
        {
            case AttackHitbox.LeftHand:
                leftHandCollider.enabled = true;
                break;

			case AttackHitbox.RightHand:
				rightHandCollider.enabled = true;
				break;

			case AttackHitbox.LeftFoot:
				leftFootCollider.enabled = true;
				break;

			case AttackHitbox.RightFoot:
				rightFootCollider.enabled = true;
				break;

			case AttackHitbox.Sword:
				swordCollider.enabled = true;
				break;

            default:
                break;
		}
    }

    private void DisableAllHitboxes()
    {
		if (swordCollider != null) swordCollider.enabled = false;
		if (leftHandCollider != null) leftHandCollider.enabled = false;
		if (rightHandCollider != null) rightHandCollider.enabled = false;
		if (leftFootCollider != null) leftFootCollider.enabled = false;
		if (rightFootCollider != null) rightFootCollider.enabled = false;
	}

	public void TryToAttack(MeeleFighter target = null) // ?붙여서 null 삽입가능하게함
    {
        if (!InAction)
        {
            StartCoroutine(Attack(target));
        }
        else if (AttackState == AttackStates.Impact || AttackState == AttackStates.Cooldown)
        {
            doCombo = true;
        }
    }

    MeeleFighter currTarget;
    private IEnumerator Attack(MeeleFighter target = null)
    {
		InAction = true;
        currTarget = target;
        AttackState = AttackStates.Windup;

        var attack = attacks[comboCount];
        var attackDir = transform.forward;
        Vector3 startPos = transform.forward;
        Vector3 targetPos = Vector3.zero;

        if (target != null)
        {
            var vecToTarget = target.transform.position - transform.position;
            vecToTarget.y = 0;

            attackDir = vecToTarget.normalized;
            float distance = vecToTarget.magnitude - attack.DistanceFromTarget;
            if (distance > longRangeAttackThreshold && longRangeAttacks.Count > 0)
                attack = longRangeAttacks[0];

            if (attack.MoveToTarget)
            {
                if (distance <= attack.MaxMoveDistance)
                    targetPos = target.transform.position - attackDir * attack.DistanceFromTarget;
                else
                    targetPos = startPos + attackDir * attack.MaxMoveDistance;
            }
        }

        animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            if (IsTakingHit) break;

            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (target != null && attack.MoveToTarget)
            {
                float percTime = (normalizedTime - attack.MoveStartTime) / (attack.MoveEndTime - attack.MoveStartTime);
                transform.position = Vector3.Lerp(startPos, targetPos, percTime);
            }

            if (attackDir != null)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir), rotationSpeed * Time.deltaTime);
            }
            
            if (AttackState == AttackStates.Windup)
            {
                if (InCounter) break;

				if (normalizedTime >= attack.ImpactStartTime)
				{
                    AttackState = AttackStates.Impact;
                    EnableHitbox(attack);
				}
			}
            else if (AttackState == AttackStates.Impact)
            {
                if (normalizedTime >= attack.ImpactEndTime)
                {
                    AttackState = AttackStates.Cooldown;
					DisableAllHitboxes();
				}
            }
            else if (AttackState == AttackStates.Cooldown)
            {
                if (doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;
                    StartCoroutine(Attack(target));
                    yield break;
                }
            }
            
            yield return null;
        }

        AttackState = AttackStates.Idle;
        comboCount = 0;
        InAction = false;
        currTarget = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Hitbox" && !IsTakingHit && !InCounter)
        {
            var attacker = other.GetComponentInParent<MeeleFighter>();
            if (attacker.currTarget != this) return;
            TakeDamage(5f);

            if (Health > 0)
                StartCoroutine(PlayHitReaction(attacker));
            else
                PlayDeathAnimation(attacker);
        }
	}

    private void TakeDamage(float damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, Health);
    }

    private void PlayDeathAnimation(MeeleFighter attacker)
    {
        animator.CrossFade("FallBackDeath", 0.2f);
    }

	private IEnumerator PlayHitReaction(MeeleFighter attacker)
	{
		InAction = true;
        IsTakingHit = true;

		var dispVec = attacker.transform.position - transform.position;
        dispVec.y = 0;
        transform.rotation = Quaternion.LookRotation(dispVec);

        OnGotHit?.Invoke(attacker);

		animator.CrossFade("SwordImpact", 0.2f);
		yield return null;
		var animState = animator.GetNextAnimatorStateInfo(1);
		yield return new WaitForSeconds(animState.length * 0.8f);
        OnHitComplete?.Invoke();
		InAction = false;
        IsTakingHit = false;
	}

	public IEnumerator PerformCounterAttack(EnemyController opponent)
    {
        InAction = true;
        InCounter = true;
        opponent.Fighter.InCounter = true;
        opponent.ChangeState(EnemyStates.Dead);

        var dispVec = opponent.transform.position - transform.position;
        dispVec.y = 0f;
        transform.rotation = Quaternion.LookRotation(dispVec);
        opponent.transform.rotation = Quaternion.LookRotation(-dispVec);

        var targetPos = opponent.transform.position - dispVec.normalized * 1f;

        animator.CrossFade("CounterAttack", 0.2f);
        opponent.Animator.CrossFade("CounterAttackVictim", 0.2f);
        yield return null;

        // 해당 레이어인덱스에서 전환중인 애니메이션의 정보
        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5 * Time.deltaTime);
            yield return null;
            timer += Time.deltaTime;
        }

		InCounter = false;
        opponent.Fighter.InCounter = false;
		InAction = false;
    }
}