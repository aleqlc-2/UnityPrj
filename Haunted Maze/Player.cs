using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;
    
    private Animator anim;
    private bool isMoving;
    private bool canMove = true;

    private float horizontalMovement;
    private float verticalMovement;
    private Vector3 direction;

    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    public AudioSource footsteps;
    public AudioClip screamSFX;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (canMove)
        {
            MovementCheck();
            AnimationCheck();
        }
    }

    private void MovementCheck()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontalMovement, 0, verticalMovement);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            controller.Move(direction * speed * Time.deltaTime);
        }
    }

    private void AnimationCheck()
    {
        if (direction != Vector3.zero && !isMoving) // 방향키 누름
        {
            footsteps.Play();
            isMoving = true;
            anim.SetBool("isRunning", isMoving);
        }
        else if (direction == Vector3.zero && isMoving) // 방향키 뗌
        {
            footsteps.Stop();
            isMoving = false;
            anim.SetBool("isRunning", isMoving);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            canMove = false;
            AudioManager.instance.PlaySFX(screamSFX);
            anim.SetTrigger("isDead");
            UIManager.instance.ShowGameOver(false); // Lose이므로 false던지면서 호출
        }
    }
}
