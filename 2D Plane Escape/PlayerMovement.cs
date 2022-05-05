using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player의 리지드바디2D Body Type을 Kinematic으로 하여 중력영향받지않도록 함
// Body Type을 Dynamic으로 하면 중력 적용되어 아래로 떨어짐
public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 200f;

    private float horizontal;

    public GameObject explosion;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); // 좌우방향키로만 이동

        // Space.World로 하면 계속 Vector2.up방향으로 수직으로만 올라가고 좌우키누르면 그 수직 라인하에서만 회전함
        // Space.Self로 해야 비행루트가 바뀜
        // Translate함수는 이동할 증분 y값만 필요하므로 Vector2.up해서 이동한거고 rb.velocity일때는 transform.up으로 이동해야함
        // 또한, 증분이동이므로 Time.deltaTime곱해서 값을 낮춰줌. 안곱하면 엄청빨리가버림
        transform.Translate(Vector2.up * speed * Time.deltaTime, Space.Self); // 전진은 자동으로 적용되게끔

        transform.Rotate(Vector3.forward * -horizontal * rotationSpeed * Time.deltaTime);
    }
}
