using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilteredPlaneCanvas : MonoBehaviour
{
    [SerializeField] private Toggle verticalPlaneToggle;
    [SerializeField] private Toggle horizontalPlaneToggle;
    [SerializeField] private Toggle bigPlaneToggle;

    [SerializeField] private Button startButton;

    private ARFilteredPlanes arFilteredPlanes;

    public bool VerticalPlaneToggle
    {
        get => verticalPlaneToggle.isOn;
        set
        {
            verticalPlaneToggle.isOn = value;
            CheckIfAllAreTrue();
        }
    }

    public bool HorizontalPlaneToggle
    {
        get => horizontalPlaneToggle.isOn;
        set
        {
            horizontalPlaneToggle.isOn = value;
            CheckIfAllAreTrue();
        }
    }

    public bool BigPlaneToggle
    {
        get => bigPlaneToggle.isOn;
        set
        {
            bigPlaneToggle.isOn = value;
            CheckIfAllAreTrue();
        }
    }

    private void OnEnable()
    {
        arFilteredPlanes = FindObjectOfType<ARFilteredPlanes>();

        arFilteredPlanes.OnVerticalPlaneFound += () => VerticalPlaneToggle = true;
        arFilteredPlanes.OnHorizontalPlaneFound += () => HorizontalPlaneToggle = true;
        arFilteredPlanes.OnBigPlaneFound += () => BigPlaneToggle = true;
    }

    private void OnDisable()
    {
        arFilteredPlanes.OnVerticalPlaneFound -= () => VerticalPlaneToggle = true;
        arFilteredPlanes.OnHorizontalPlaneFound -= () => HorizontalPlaneToggle = true;
        arFilteredPlanes.OnBigPlaneFound -= () => BigPlaneToggle = true;
    }
    
    private void CheckIfAllAreTrue()
    {
        if (VerticalPlaneToggle && HorizontalPlaneToggle && BigPlaneToggle)
            startButton.interactable = true;
    }
}
