using UnityEngine;


public class Drive : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 100f;

    public GameObject fuel;
    Vector3 direction;

    void Start()
    {
        direction = fuel.transform.position - this.transform.position;
        Coords dirNormal = HolisticMath.GetNormal(new Coords(direction));
        direction = dirNormal.ToVector();
        float a = HolisticMath.Angle(new Coords(this.transform.up), new Coords(direction)); // this.transform.up는 Vector3(0,1,0)

        // 탱크의 y축과 fuel까지의 방향축의 외적한값의 z가 0보다 작으면 360도에서 해당 각도를 빼서 방향을 반대로
        bool clockwise = false;
        if (HolisticMath.Cross(new Coords(this.transform.up), dirNormal).z < 0)
        {
            clockwise = true;
        }

        Coords newDir = HolisticMath.Rotate(new Coords(this.transform.up), a, clockwise);

        // transform.up이 y축인데 값을 직접변화시켰으므로 에디터상에서 transform.rotation.z 값이 변하면서 tank 회전함
        this.transform.up = new Vector3(newDir.x, newDir.y, newDir.z);
    }

	private void Update()
	{
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

		transform.position = HolisticMath.Translate(new Coords(transform.position), new Coords(transform.up), new Coords(0, translation, 0)).ToVector();
        transform.up = HolisticMath.Rotate(new Coords(transform.up), rotation * Mathf.Deg2Rad, true).ToVector();
	}
}