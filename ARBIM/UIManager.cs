using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private GameObject buildingOnPlaneWithLight;
    public GameObject BuildingOnPlaneWithLight
    {
		get { return buildingOnPlaneWithLight; }
		set { buildingOnPlaneWithLight = value; }
	}

    [SerializeField] private GameObject ScrollView;
    [SerializeField] private Button btnShowScrollView;

    [SerializeField] private TextMeshProUGUI planeClickText;

    private Transform[] childrenOnPlaneWithLight;

    private bool isCreatedBuilding = false;
    public bool IsCreatedBuilding
    {
        get { return isCreatedBuilding; }
        set { isCreatedBuilding = value; }
    }

    private Coroutine coroutine;
    public Coroutine Coroutine
    {
        get { return coroutine; }
        set { coroutine = value; }
    }

    private TextMeshProUGUI fpsLogTxt;
    public TextMeshProUGUI FpsLogTxt
	{
		get { return fpsLogTxt; }
		set { fpsLogTxt = value; }
	}

    //// 디버깅텍스트
    //public TextMeshProUGUI touchBeganTxt;
    //public TextMeshProUGUI touch0Txt;
    //public TextMeshProUGUI touch1Txt;
    //public TextMeshProUGUI touchMovedTxt;

    void Awake()
    {
        instance = this;
    }

    public void InitializeBuildingOnPlaneWithLight()
    {
        childrenOnPlaneWithLight = buildingOnPlaneWithLight.transform.GetComponentsInChildren<Transform>(); // root도 포함
    }

    public void OnClickPanelButton()
    {
        foreach (var child in childrenOnPlaneWithLight)
        {
            if (child.gameObject.name == EventSystem.current.currentSelectedGameObject.gameObject.transform.parent.name)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                    EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    child.gameObject.SetActive(true);
                    EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Image>().color = Color.blue;
                }
            }
        }
    }

    public void StartCorouTextBlink()
    {
        coroutine = StartCoroutine(TextBlink());
    }

    private IEnumerator TextBlink()
    {
        while (!isCreatedBuilding)
        {
            planeClickText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            planeClickText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void HideScrollView()
    {
        ScrollView.SetActive(false);
        btnShowScrollView.gameObject.SetActive(true);
    }

    public void ShowScrollView()
    {
        ScrollView.SetActive(true);
        btnShowScrollView.gameObject.SetActive(false);
    }
}
