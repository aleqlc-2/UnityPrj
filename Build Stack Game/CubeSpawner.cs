using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection
{
    X,
    Z
}

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private MovingCube cubePrefab; // 스크립트를..
    [SerializeField] private MoveDirection moveDirection; // 인스펙터창에서 X,Z 선택

    public void SpawnCube()
    {
        var cube = Instantiate(cubePrefab); // 생성하다니..

        // 이전 생성된 큐브와 x, z 위치를 맞추기 위해
        if (MovingCube.LastCube != null && MovingCube.LastCube.gameObject != GameObject.Find("Start"))
        {
            float x = moveDirection == MoveDirection.X ?
                             transform.position.x : MovingCube.LastCube.transform.position.x;

            float z = moveDirection == MoveDirection.Z ?
                             transform.position.z : MovingCube.LastCube.transform.position.z;

            cube.transform.position = new Vector3(
                                x,
                                MovingCube.LastCube.transform.position.y + cubePrefab.transform.localScale.y,
                                z);
        }
        else // 첫 생성일때
        {
            cube.transform.position = transform.position;
        }

        cube.MoveDirection = moveDirection; // MovingCube스크립트에서 구분해서 사용하기위해
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireCube(transform.position, cubePrefab.transform.localScale);
    // }
}