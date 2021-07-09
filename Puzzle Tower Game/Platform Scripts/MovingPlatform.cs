using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform movePoint;

    [SerializeField] private float smoothMovement = 0.3f;
    [SerializeField] private float halfDistance = 15f;
    [SerializeField] private float timer = 1f;
    
    [SerializeField] private bool activateMovementInStart; // scene2 시작하자마자 이동
    [SerializeField] private bool deactivateDoors;

    private DoorController doorController;

    private float initialMovement;

    private Vector3 startPosition;

    private bool smoothMovementHalfed;
    private bool can_Move;
    private bool move_To_Initial;

    private PlatformSoundFX soundFX;

    private RotatingPlatform rotatePlatform;
    
    // scene2 의 white button platform
    [SerializeField] private bool activateRotation;

    void Awake()
    {
        startPosition = transform.position;
        initialMovement = smoothMovement;

        // activate doors
        doorController = GetComponent<DoorController>();

        // add sound
        soundFX = GetComponent<PlatformSoundFX>();

        if (activateRotation)
            rotatePlatform = GetComponent<RotatingPlatform>();
    }

    void Start()
    {
        if (activateMovementInStart)
        {
            Invoke("ActivateMovement", timer);
        }
    }

    void Update()
    {
        MovePlatform();
        MoveToInitialPosition(); // scene2
    }

    void MovePlatform()
    {
        if (can_Move)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, movePoint.position, smoothMovement);

            // halfDistance거리 안으로 들어오면 이동속도를 절반으로 줄임
            if (Vector3.Distance(transform.position, movePoint.position) <= halfDistance)
            {
                if (!smoothMovementHalfed)
                {
                    smoothMovement /= 2f;
                    smoothMovementHalfed = true;
                }
            }

            // 시작시 플랫폼이 도착하면
            if (Vector3.Distance(transform.position, movePoint.position) == 0f)
            {
                can_Move = false;

                if (smoothMovementHalfed)
                {
                    smoothMovement = initialMovement; // 이동속도 초기화
                    smoothMovementHalfed = false; // 초기화
                }

                if (deactivateDoors)
                {
                    doorController.OpenDoors();
                }

                // stop playing the sound FX
                soundFX.PlayAudio(false);
            }
        }
    }

    public void ActivateMovement()
    {
        can_Move = true;

        // play sound fx
        soundFX.PlayAudio(true);

        // rotate
        if (activateRotation)
        {
            rotatePlatform.ActivateRotation();
        }
    }

    public void ActivateMoveToInitial()
    {
        move_To_Initial = true;
        soundFX.PlayAudio(true);
    }

    void MoveToInitialPosition()
    {
        if (move_To_Initial)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, startPosition, smoothMovement);

            if (Vector3.Distance(transform.position, startPosition) <= halfDistance)
            {
                if (!smoothMovementHalfed)
                {
                    smoothMovement /= 2f;
                    smoothMovementHalfed = true;
                }
            }

            if (Vector3.Distance(transform.position, startPosition) == 0f)
            {
                move_To_Initial = false;

                if (smoothMovementHalfed)
                {
                    smoothMovementHalfed = false;
                    smoothMovement = initialMovement;
                }

                soundFX.PlayAudio(false);
            }
        }
    }
}
