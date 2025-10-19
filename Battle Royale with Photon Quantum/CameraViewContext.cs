using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;
using Cinemachine;

public class CameraViewContext : MonoBehaviour, IQuantumViewContext
{
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
}
