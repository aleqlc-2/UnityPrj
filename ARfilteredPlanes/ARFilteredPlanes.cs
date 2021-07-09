using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Linq;
using System;

public class ARFilteredPlanes : MonoBehaviour
{
    // 인스펙터창에서 x, y 값 할당했음
    [SerializeField] private Vector2 dimensionsForBigPlane;

    // Action => using System;
    public event Action OnVerticalPlaneFound;
    public event Action OnHorizontalPlaneFound;
    public event Action OnBigPlaneFound;

    private ARPlaneManager arPlaneManager;
    private List<ARPlane> arPlanes;

    private void OnEnable()
    {
        arPlanes = new List<ARPlane>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added != null && args.added.Count > 0)
            arPlanes.AddRange(args.added);
        
        // Where => using System.Linq;
        foreach (ARPlane plane in arPlanes.Where(plane => plane.extents.x * plane.extents.y >= 0.1f))
        {
            // IsVertical => using UnityEngine.XR.ARSubsystems;
            if (plane.alignment.IsVertical())
            {
                // tell someone we found a vertical plane
                OnVerticalPlaneFound.Invoke();
            }
            else
            {
                // tell someone we found a horizontal plane
                OnHorizontalPlaneFound.Invoke();
            }

            if (plane.extents.x * plane.extents.y >= dimensionsForBigPlane.x * dimensionsForBigPlane.y)
            {
                // tell someone we found a big plane
                OnBigPlaneFound.Invoke();
            }
        }
    }
}
