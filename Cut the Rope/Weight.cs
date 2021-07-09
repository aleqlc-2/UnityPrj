using UnityEngine;

public class Weight : MonoBehaviour
{
    public float distanceFromChainEnd = 0.6f;

    public void ConnectRopeEnd(Rigidbody2D endRB)
    {
        HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false; //내가 정한 수치대로 anchor하기위해
        joint.connectedBody = endRB;
        joint.anchor = Vector2.zero;

        // - 안붙여도 똑같은데
        joint.connectedAnchor = new Vector2(0f, -distanceFromChainEnd); // Weight와 거리설정
    }
}
