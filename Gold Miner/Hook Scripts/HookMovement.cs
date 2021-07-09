using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookMovement : MonoBehaviour
{
    //rotation Z
    public float min_Z = -55f, max_Z = 55f;
    public float rotate_Speed = 5f;

    private float rotate_Angel;
    private bool rotate_Right;
    private bool canRotate;

    public float move_Speed = 3f;
    private float initial_Move_Speed;

    public float min_Y = -2.5f;
    private float initial_Y;

    private bool moveDown;

    //For Line Renderer
    private RopeRenderer ropeRenderer;

    void Awake()
    {
        ropeRenderer = GetComponent<RopeRenderer>();
    }

    void Start()
    {
        initial_Y = transform.position.y;
        initial_Move_Speed = move_Speed;

        canRotate = true;
    }
    
    void Update()
    {
        Rotate();
        GetInput();
        MoveRope();
    }

    void Rotate()
    {
        if (!canRotate)
            return;

        if (rotate_Right)
        {
            rotate_Angel += rotate_Speed * Time.deltaTime;
        }
        else
        {
            rotate_Angel -= rotate_Speed * Time.deltaTime;
        }

        transform.rotation = Quaternion.AngleAxis(rotate_Angel, Vector3.forward);

        if (rotate_Angel >= max_Z)
        {
            rotate_Right = false;
        }
        else if (rotate_Angel <= min_Z)
        {
            rotate_Right = true;
        }
    }

    void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canRotate)
            {
                canRotate = false;
                moveDown = true;
            }
        }
    }

    void MoveRope()
    {
        if (canRotate) // 클릭시 내려갔다 올라올때 temp.y >= initial_Y 되면 Hook에서 멈추도록
            return;

        if (!canRotate)
        {
            //SoundManager.instance.RopeStretch(true);

            Vector3 temp = transform.position;

            if (moveDown)
            {
                temp -= transform.up * Time.deltaTime * move_Speed;
                // temp.y -= Time.deltaTime * move_Speed; // 이렇게 하면 수직으로만 내려감
            }
            else
            {
                temp += transform.up * Time.deltaTime * move_Speed;
                // temp.y += Time.deltaTime * move_Speed; // 이렇게 하면 수직으로만 올라감
            }

            transform.position = temp;

            if (temp.y <= min_Y)
            {
                moveDown = false;
            }

            if (temp.y >= initial_Y)
            {
                canRotate = true;

                // deactivate line renderer
                ropeRenderer.RenderLine(temp, false);

                // reset move speed
                move_Speed = initial_Move_Speed;
            }
            
            ropeRenderer.RenderLine(temp, true);
        }
    }
}
