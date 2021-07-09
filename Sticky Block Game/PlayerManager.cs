using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // 인스펙터창에서 선택
    public PlayerState playerState;
    public enum PlayerState
    {
        Stop,
        Move
    }

    // 인스펙터창에서 선택
    public LevelState levelState;
    public enum LevelState
    {
        NotFinished,
        Finished
    }

    public Material collectedObjMat;

    public List<GameObject> collidedList;

    public Transform collectedPoolTransform;
    public Transform particlePrefab;

    public void CallMakeSphere()
    {
        foreach (GameObject obj in collidedList)
        {
            obj.GetComponent<CollectedObjController>().MakeShpere();
        }
    }
}
