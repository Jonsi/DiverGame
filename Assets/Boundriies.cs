using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundriies : MonoBehaviour
{
    [Header("Boundaries")]
    public Transform TopClamp;
    public Transform LeftClamp;
    public Transform BottomClamp;
    public Transform RightClamp;

    public Vector3 ClampPosToBounderies(Vector3 pos)
    {
        float xPos = Mathf.Clamp(pos.x, LeftClamp.position.x, RightClamp.position.x);
        float yPos = Mathf.Clamp(pos.y, BottomClamp.position.y, TopClamp.position.y);
        return new Vector3(xPos, yPos,pos.z);
    }
}
