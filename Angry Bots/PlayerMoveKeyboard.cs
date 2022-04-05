using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveKeyboard : MonoBehaviour
{
    private FreeMovementMotor motor;
    private Transform player;
    public GameObject cursorPrefab;

    public float cameraSmoothing = 0.05f;
    public float cameraPreview = 2f;

    public float cursor_PlaneHeight;
    public float cursor_FacingCamera = 1f;
    public float cursor_SmallerWithDistance = 0f;
    public float cursor_SmallerWhenClose = 1f;

    private Camera mainCamera;
    private Vector3 mainCamera_Velocity;
    private Vector3 mainCamera_Offset;
    private Vector3 initOffsetToPlayer;

    private Transform cursorObject;
    private Vector3 cursorScreenPosition;

    private Quaternion screenMovement_Space;
    private Vector3 screenMovement_Right;
    private Vector3 screenMovement_Forward;

    private Plane playerMovementPlane;

    void Awake()
    {
        motor = GetComponent<FreeMovementMotor>();
        motor.movementDirection = Vector3.zero;
        motor.facingDirection = Vector3.zero;

        mainCamera = Camera.main;

        cursorObject = (Instantiate(cursorPrefab) as GameObject).transform;

        player = transform;

        initOffsetToPlayer = mainCamera.transform.position - player.position;
        mainCamera_Offset = mainCamera.transform.position - player.position;

        cursorScreenPosition = new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0f);

        // 첫번째인자는 평면의 수직방향(캐릭터의 y축방향을 위로 바라보겠다)
        // 두번째인자는 Plane이 생성될 위치
        playerMovementPlane = new Plane(player.up, player.position + player.up * cursor_PlaneHeight);
    }

    void Start()
    {
        screenMovement_Space = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
        screenMovement_Forward = screenMovement_Space * Vector3.forward;
        screenMovement_Right = screenMovement_Space * Vector3.right;
    }

    void FixedUpdate()
    {
        HanglePlayerMovement();
    }

    private void HanglePlayerMovement()
    {
        motor.movementDirection = Input.GetAxis("Horizontal") * screenMovement_Right +
                                  Input.GetAxis("Vertical") * screenMovement_Forward;

        if (motor.movementDirection.sqrMagnitude > 1) motor.movementDirection.Normalize();

        playerMovementPlane.normal = player.up;
        playerMovementPlane.distance = -player.position.y + cursor_PlaneHeight;

        Vector3 cameraAdjustmentVector = Vector3.zero;
        Vector3 cursor_ScreenPosition = Input.mousePosition;

        Vector3 cursorWorldPosition = ScreenPointToWorldPointOnPlane(cursor_ScreenPosition, playerMovementPlane, mainCamera);

        float halfWidth = Screen.width / 2f;
        float halfHeight = Screen.height / 2f;
        float maxHalf = Mathf.Max(halfWidth, halfHeight);

        // screen position에 대한 상대적 위치
        Vector3 posRel = cursor_ScreenPosition - new Vector3(halfWidth, halfHeight, cursor_ScreenPosition.z);

        posRel.x /= maxHalf;
        posRel.y /= maxHalf;

        cameraAdjustmentVector = posRel.x * screenMovement_Right + posRel.y * screenMovement_Forward;
        cameraAdjustmentVector.y = 0f;

        HandleCursorAlignment(cursorWorldPosition);

        // Handle camera
        Vector3 cameraTargetPosition = player.position + initOffsetToPlayer + cameraAdjustmentVector * cameraPreview;
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, // 현재위치
                                                          cameraTargetPosition, // 도달하려는 위치
                                                          ref mainCamera_Velocity, // 현재속도
                                                          cameraSmoothing); // 타겟에 도달하기 위한 대략적인 시간. 작은 값일수록 타겟에 빠르게 도달

        mainCamera_Offset = mainCamera.transform.position - player.position;
    }

    private Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera myCamera)
    {
        Ray ray = myCamera.ScreenPointToRay(screenPoint);

        float dist;
        plane.Raycast(ray, out dist); // Physics.Raycast와는 다르게 거리가 구해짐
        return ray.GetPoint(dist); // ray.GetPoint(dist)가 마우스로 클릭한 plane상의 지점
    }

    private void HandleCursorAlignment(Vector3 cursorWorldPosition)
    {
        cursorObject.position = cursorWorldPosition;

        Quaternion cursor_WorldRotation = cursorObject.rotation;

        if (motor.facingDirection != Vector3.zero)
            cursor_WorldRotation = Quaternion.LookRotation(motor.facingDirection);

        Vector3 cursor_ScreenSpaceDirection = Input.mousePosition -
                                              mainCamera.WorldToScreenPoint(transform.position + player.up * cursor_PlaneHeight);

        cursor_ScreenSpaceDirection.z = 0f;

        Quaternion cursor_Bilboard_Rotation = mainCamera.transform.rotation *
                                              Quaternion.LookRotation(cursor_ScreenSpaceDirection, -Vector3.forward);

        cursorObject.rotation = Quaternion.Slerp(cursor_WorldRotation, cursor_Bilboard_Rotation, cursor_FacingCamera);

        float compensatedScale = 0.1f * Vector3.Dot(cursorWorldPosition - mainCamera.transform.position, mainCamera.transform.forward);

        // Mathf.Lerp는 세번째인자값이 결과값이 되어버리는것이고
        // Mathf.InverseLerp는 세번째인자값이 첫번째인자와 두번째인자 사이에서 백분율로 몇인지 0~1사이의 값으로 리턴됨
        // Mathf.PingPong(Time.time, 3); 0부터 3까지 이동했다가 다시 0으로 이동 반복
        // Mathf.DeltaAngle(0, 90); 두 각도 사이의 차이인 90
        float cursor_Scale_Multiplier = Mathf.Lerp(0.7f, 1.0f, Mathf.InverseLerp(0.5f, 4.0f, motor.facingDirection.magnitude));

        cursorObject.localScale = Vector3.one * Mathf.Lerp(compensatedScale, 1f, cursor_SmallerWithDistance) * cursor_Scale_Multiplier;
    }

    //// CharacterController컴포넌트를 가진 객체가 다른 객체에 부딪혔을 때 호출됨
    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (!characterController.isGrounded)
    //    {
    //        if (hit.collider.tag == "Wall") // 부딪힌 대상
    //        {
    //            if (verticalVelocity < -0.6f) wallSlide = true;
                
    //            if (Input.GetKeyDown(KeyCode.Space))
    //            {
    //                verticalVelocity = jumpForce;

    //                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);

    //                wallSlide = false;
    //            }
    //        }
    //    }
    //}
}
