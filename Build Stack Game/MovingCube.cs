using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingCube : MonoBehaviour
{
    public static MovingCube CurrentCube { get; private set; }
    public static MovingCube LastCube { get; private set; }

    // CubeSpawner스크립트에서 직접적으로 값을 할당했기때문에
    // get set없어도 동작은 동일하게 됨
    public MoveDirection MoveDirection { get; set; }

    [SerializeField] private float moveSpeed = 1f;

    private void OnEnable()
    {
        // 게임이 시작되면 처음에 젤 밑에 큰 큐브의 스크립트를 LastCube로 설정하고
        // 그 다음 생성된 큐브들은 일단 젤 밑에 큰 큐브를 LastCube로 넣었다가
        // 클릭할때 Stop함수 호출에서 자기자신(this)을 LastCube로 설정
        if (LastCube == null)
            LastCube = GameObject.Find("Start").GetComponent<MovingCube>();
        // 아래 else코드 안됨. 왜냐하면 클릭했을 때 새로운 오브젝트가 생성되고
        // 새로 생성된 오브젝트에 달린 스크립트의 LastCube변수는 null이라서
        // 항상 위의 if 구문만 실행됨
        //else 
            //LastCube = this;
        
        CurrentCube = this;
        GetComponent<Renderer>().material.color = GetRandomColor();

        // 이전 큐브가 잘리고 남은 크기만큼 스폰되게 하기위해
        transform.localScale = new Vector3(
                            LastCube.transform.localScale.x,
                            transform.localScale.y, // not LastCube, y값은 인스펙터창에 입력된 0.1로 고정
                            LastCube.transform.localScale.z);
    }

    private Color GetRandomColor()
    {
        return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }

    private void Update()
    {
        if (MoveDirection == MoveDirection.Z)
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
        else // X
            transform.position += transform.right * Time.deltaTime * moveSpeed;
    }

    // internal은 같은 어셈블리 내에서는 public, 다른 어셈블리에서는 private
    internal void Stop()
    {
        moveSpeed = 0;

        // position
        float hangover = GetHangover();

        // scale
        float max = MoveDirection == MoveDirection.Z ?
                        LastCube.transform.localScale.z : LastCube.transform.localScale.x;

        // 젤 위의 큐브 범위 밖에서 클릭하면 게임 재시작
        // 덜와서 클릭한거랑 더가서 클릭한거랑 둘다 범위벗어나는것이므로 절대값으로 비교
        if (Mathf.Abs(hangover) >= max)
        {
            LastCube = null;
            CurrentCube = null;
            SceneManager.LoadScene(0);
        }
        
        // 첫 클릭에서는 젤 밑에 큰 큐브가 0,0,0 이므로 움직이는큐브의z - 0 = 움직이는큐브의z 가 출력됨
        // 다음클릭은 이전큐브의z - 움직이는큐브z 가 출력됨
        Debug.Log(hangover);

        // hangover가 음수일때는 떨어질 큐브의 위치를 반대방향으로 대칭이동 시켜줘야하기 때문에
        float direction = hangover > 0 ? 1f : -1f;

        if (MoveDirection == MoveDirection.Z)
            SplitCubeOnZ(hangover, direction);
        else
            SplitCubeOnX(hangover, direction);

        LastCube = this; // 이 코드 없으면 항상 같은 높이에서 생성됨
    }

    private float GetHangover()
    {
        // LastCube는 스크립트객체이고
        // 스크립트안에 transform값이 있기때문에 LastCube.transform.position.z 이런식의 호출 가능
        if (MoveDirection == MoveDirection.Z)
            return transform.position.z - LastCube.transform.position.z;
        else // X
            return transform.position.x - LastCube.transform.position.x;
    }

    private void SplitCubeOnX(float hangover, float direction)
    {
        float newXPosition = LastCube.transform.position.x + (hangover / 2); // 쌓일 조각의 x위치
        float newXSize = LastCube.transform.localScale.x - Mathf.Abs(hangover); // 쌓일 조각의 x사이즈
        float cubeEdge = transform.position.x + (newXSize / 2f * direction); // 조각난 경계지점
        float fallingBlockSize = transform.localScale.x - newXSize; // 떨어질 조각의 x사이즈
        float fallingBlockXPosition = cubeEdge + (fallingBlockSize / 2f * direction); // 떨어질 조각의 x위치

        // 현재 큐브를 쌓일 조각의 위치와 크기로 변경
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
        transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
        
        // sphere가 조각난 경계선 사이에 생성됨(테스트 용도임)
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(cubeEdge, transform.position.y, transform.position.z);
        sphere.transform.localScale = Vector3.one * 0.1f;

        // 떨어질 조각 새로 생성
        SpawnDropCube(fallingBlockXPosition, fallingBlockSize);
    }

    private void SplitCubeOnZ(float hangover, float direction)
    {
        float newZPosition = LastCube.transform.position.z + (hangover / 2);
        float newZSize = LastCube.transform.localScale.z - Mathf.Abs(hangover);
        float cubeEdge = transform.position.z + (newZSize / 2f * direction);
        float fallingBlockSize = transform.localScale.z - newZSize;
        float fallingBlockZPosition = cubeEdge + (fallingBlockSize / 2f * direction);
        
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
        
        // sphere가 조각난 경계선 사이에 생성됨(테스트 용도임)
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(transform.position.x, transform.position.y, cubeEdge);
        sphere.transform.localScale = Vector3.one * 0.1f;

        SpawnDropCube(fallingBlockZPosition, fallingBlockSize);
    }

    private void SpawnDropCube(float fallingBlockPosition, float fallingBlockSize)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        if (MoveDirection == MoveDirection.Z)
        {
            cube.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockPosition);
            cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingBlockSize);
        }
        else
        {
            cube.transform.position = new Vector3(fallingBlockPosition, transform.position.y, transform.position.z);
            cube.transform.localScale = new Vector3(fallingBlockSize, transform.localScale.y, transform.localScale.z);
        }
        
        cube.AddComponent<Rigidbody>();

        // 원래 큐브의 색깔을 새로 생성된 떨어질 큐브에도 적용
        cube.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;

        Destroy(cube.gameObject, 1f);
    }
}
