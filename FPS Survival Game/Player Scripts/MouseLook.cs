using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform playerRoot, lookRoot;
    [SerializeField] private bool invert;
    [SerializeField] private bool can_Unlock = true;
    [SerializeField] private float sensivity = 5f;
    [SerializeField] private int smooth_Steps = 10;
    [SerializeField] private float smooth_Weight = 0.4f;
    [SerializeField] private float roll_Angle = 10f;
    [SerializeField] private float roll_Speed = 3f;
    [SerializeField] private Vector2 default_Look_Limits = new Vector2(-70f, 80f);

    private Vector2 look_Angles;
    private Vector2 current_Mouse_Look;
    private Vector2 smooth_Move;
    private float current_Roll_Angle;
    private int last_Look_Frame;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 게임상에서 커서가 안보이도록함, 설명참고
    }

    void Update()
    {
        LockAndUnlockCursor();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            LookAround();
        }
    }

    void LockAndUnlockCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void LookAround()
    {
        // MOUSE_Y(y축 위를 움직임)가 상하움직임, MOUSE_X(x축 위를 움직임)가 좌우움직임
        current_Mouse_Look = new Vector2(Input.GetAxis(MouseAxis.MOUSE_Y), Input.GetAxis(MouseAxis.MOUSE_X));

        // -1f 안곱하면 상하 거꾸로 움직임
        look_Angles.x += current_Mouse_Look.x * sensivity * (invert ? 1f : -1f); // 상하
        look_Angles.y += current_Mouse_Look.y * sensivity; // 좌우

        // 좌우로는 360도 회전가능하지만 상하로는 제한을 두어야하기때문에
        look_Angles.x = Mathf.Clamp(look_Angles.x, default_Look_Limits.x, default_Look_Limits.y);

        // 근데 딱히 기울임 적용 안해도 동작차이가 안느껴짐
        // 현재 기울임에서 좌우움직인 기울임으로 Lerp를 사용하여 값 변경
        current_Roll_Angle = Mathf.Lerp(current_Roll_Angle, Input.GetAxisRaw(MouseAxis.MOUSE_X)
                                        * roll_Angle, Time.deltaTime * roll_Speed);
        // 상하움직임은 x축을 기준으로, 기울임 각도는 z축(정면)을 기준으로 current_Roll_Angle
        lookRoot.localRotation = Quaternion.Euler(look_Angles.x, 0f, current_Roll_Angle);

        // 좌우움직임은 y축을 기준으로
        playerRoot.localRotation = Quaternion.Euler(0f, look_Angles.y, 0f);
    }
}
