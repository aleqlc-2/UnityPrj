using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] // 구조체나 클래스를 인스펙터창에 노출시킴, MonoBehaviour 상속받지 않는 클래스에만 사용가능
public class Level
{
    // Range 어트리뷰트는 인스펙터창에서 슬라이드바를 이용하여 값을 설정하게 해줌
    [Range(1, 11)] public int partCount; // 1~11
    [Range(0, 11)] public int deathPartCount; // 0~11
}

// Stage 파일 생성가능
[CreateAssetMenu(fileName = "New Stage")] // ScriptableObject를 상속받은 클래스를 .asset 파일로 저장
public class Stage : ScriptableObject // 객체에 붙이지 않고 사용, 데이터 저장 목적
{
    public Color stageBackgroundColor = Color.white;
    public Color stageLevelPartColor = Color.white;
    public Color stageBallColor = Color.white;
    public List<Level> levels = new List<Level>();
}
