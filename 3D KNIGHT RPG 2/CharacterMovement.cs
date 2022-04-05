using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private MovementMotor motor;

    public float move_Magnitude = 0.05f;
    public float speed = 0.7f;
    public float speed_Move_WhileAttack = 0.1f;
    public float speed_Attack = 1.5f;
    public float turnSpeed = 10f;
    public float speed_Jump = 20f;

    private float speed_Move_Multiplier = 1f;

    private Vector3 direction;
    public Vector3 MoveDirection
    {
        get { return direction; }
        set
        {
            direction = value * speed_Move_Multiplier;

            if (direction.magnitude > 0.1f)
            {
                var newRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turnSpeed);
            }

            direction *= speed * (Vector3.Dot(transform.forward, direction) + 1f) * 5f;
            motor.Move(direction);

            AnimationMove(motor.charController.velocity.magnitude * 0.1f);
        }
    }

    private Animator anim;
    private Camera mainCamera;

    // Animator탭에서 정한 파라미터들
    private string PARAMETER_STATE = "State";
    private string PARAMETER_ATTACK_TYPE = "AttackType";
    private string PARAMETER_ATTACK_INDEX = "AttackIndex";

    public AttackAnimation[] attack_Animations;
    public string[] combo_AttackList;
    public int combo_Type;

    private int attack_Index = 0;
    private string[] combo_List;
    private int attack_Stack;
    private float attack_Stack_TimeTemp;

    private bool isAttacking;

    private GameObject atkPoint;

    void Awake()
    {
        motor = GetComponent<MovementMotor>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        anim.applyRootMotion = false; // 이 코드없으면 애니메이션이 idle에서 다른데로 전이가 안됨
        mainCamera = Camera.main;
        atkPoint = GameObject.Find("Player Attack Point");
        atkPoint.SetActive(false);
    }

    void Update()
    {
        HandleAttackAnimations();

        if (MouseLock.MouseLocked)
        {
            if (Input.GetButtonDown("Fire1")) Attack();
            if (Input.GetButtonDown("Fire2")) Attack();
        }

        MovementAndJumping();
    }

    // ?
    private void MovementAndJumping()
    {
        Vector3 moveInput = Vector3.zero;
        Vector3 forward = Quaternion.AngleAxis(-90f, Vector3.up) * mainCamera.transform.right; // 캐릭터프리펩이 y축기준 -90도 회전되어있음

        moveInput += forward * Input.GetAxis("Vertical");
        moveInput += mainCamera.transform.right * Input.GetAxis("Horizontal");
        moveInput.Normalize();

        Moving(moveInput.normalized, 1f);

        if (Input.GetKey(KeyCode.Space)) Jump();
    }

    private void Moving(Vector3 dir, float mult)
    {
        if (isAttacking)
            speed_Move_Multiplier = speed_Move_WhileAttack * mult;
        else
            speed_Move_Multiplier = 1 * mult;

        MoveDirection = dir;
    }

    private void Jump()
    {
        motor.Jump(speed_Jump);
    }

    private void AnimationMove(float magnitude)
    {
        if (magnitude > move_Magnitude)
        {
            float speed_Animation = magnitude * 2f;
            if (speed_Animation < 1f) speed_Animation = 1f;
            if (anim.GetInteger(PARAMETER_STATE) != 2) // attack중이 아니라면
            {
                anim.SetInteger(PARAMETER_STATE, 1); // run
                anim.speed = speed_Animation;
            }
        }
        else
        {
            if (anim.GetInteger(PARAMETER_STATE) != 2) // attack중이 아니라면
            {
                anim.SetInteger(PARAMETER_STATE, 0); // idle
            }
        }
    }

    private void HandleAttackAnimations()
    {
        if (Time.time > attack_Stack_TimeTemp + 0.5f)
        {
            attack_Stack = 0;
        }

        // 인스펙터창에서 combo_AttackList[combo_Type]에 0,1 적음
        // 0,1 에서 ,를 제거하고 0 1 을 combo_List에 저장함
        // 배열로 저장되므로 foreach돌려서 찍어보면 0과 1 출력됨
        // [0]은 ","안에 있는 첫번째 문자인 ,로 분리하겠다.
        combo_List = combo_AttackList[combo_Type].Split("," [0]);

        if (anim.GetInteger(PARAMETER_STATE) == 2)
        {
            anim.speed = speed_Attack;

            // 현재 진행되고 있는 애니메이션 정보를 가져옴
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsTag("Attack")) // Animator탭에서 각각의 State의 인스펙터창에서 Tag달 수 있음 
            {
                int motionIndex = int.Parse(combo_List[attack_Index]);

                // normalizedTime이 0이면 애니메이션이 아직 시작되지 않음
                // normalizedTime이 1이면 애니메이션이 모두 진행되었음
                // 즉, 애니메이션이 90%이상 진행되었다면
                if (stateInfo.normalizedTime > 0.9f)
                {
                    anim.SetInteger(PARAMETER_STATE, 0);
                    isAttacking = false;
                    attack_Index++;

                    if (attack_Stack > 1)
                    {
                        FightAnimation();
                    }
                    else
                    {
                        if (attack_Index >= combo_List.Length)
                        {
                            ResetCombo();
                        }
                    }
                }
            }
        }
    }
    
    private void Attack()
    {
        if (attack_Stack < 1 || (Time.time > attack_Stack_TimeTemp + 0.2f && Time.time < attack_Stack_TimeTemp + 1f))
        {
            attack_Stack++;
            attack_Stack_TimeTemp = Time.time;
        }

        FightAnimation();
    }

    private void FightAnimation()
    {
        if (combo_List != null && attack_Index >= combo_List.Length)
        {
            ResetCombo();
        }

        if (combo_List != null && combo_List.Length > 0)
        {
            int motionIndex = int.Parse(combo_List[attack_Index]);

            if (motionIndex < attack_Animations.Length)
            {
                anim.SetInteger(PARAMETER_STATE, 2);
                anim.SetInteger(PARAMETER_ATTACK_TYPE, combo_Type);
                anim.SetInteger(PARAMETER_ATTACK_INDEX, attack_Index);
            }
        }
    }

    private void ResetCombo()
    {
        attack_Index = 0;
        attack_Stack = 0;
        isAttacking = false;
    }

    private void Attack_Began()
    {
        atkPoint.SetActive(true);
    }

    private void Attack_End()
    {
        atkPoint.SetActive(false);
    }
}
