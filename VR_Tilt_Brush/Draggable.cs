using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Draggable : MonoBehaviour
{
    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Action_Boolean trigger;

    public bool fixX;
    public bool fixY;
    public Transform thumb;

    private bool dragging;

    void FixedUpdate()
    {
        if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            dragging = false;
            Ray ray = new Ray(trackedObj.transform.position, trackedObj.transform.forward);
            RaycastHit hit;
            if (GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                dragging = true;
            }
        }

        if (trigger.GetState(SteamVR_Input_Sources.RightHand))
        {
            Ray ray = new Ray(trackedObj.transform.position, trackedObj.transform.forward);
            RaycastHit hit;
            if(GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                var point = hit.point;
                point = GetComponent<Collider>().ClosestPointOnBounds(point);
                SetThumbPosition(point);
                SendMessage("OnDrag",
                 Vector3.one - (thumb.position - GetComponent<Collider>().bounds.min) / GetComponent<Collider>().bounds.size.x);
            }
        }

        if(trigger.GetStateUp(SteamVR_Input_Sources.RightHand)) dragging = false;

        void SetThumbPosition(Vector3 point)
        {
            thumb.position = new Vector3
                (fixX ? thumb.position.x : point.x, fixY ? thumb.position.y : point.y, thumb.position.z);
        }

        void SetDragPoint(Vector3 point)
        {
            point = (Vector3.one - point)
                     * GetComponent<Collider>().bounds.size.x + GetComponent<Collider>().bounds.min;
            SetThumbPosition(point);
        }
    }
}
