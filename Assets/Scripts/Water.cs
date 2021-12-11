using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [Header("Boundaries")]
    public Transform TopClamp;
    public Transform LeftClamp;
    public Transform BottomClamp;
    public Transform RightClamp;

    private Vector3 _clampedPos;

    void Update()
    {
        _clampedPos = PlayerController.Singleton.transform.position;
        //_clampedPos.x = Mathf.Clamp(transform.position.x, LeftClamp.position.x, RightClamp.position.x);
        _clampedPos.y = Mathf.Clamp(PlayerController.Singleton.transform.position.y, BottomClamp.position.y, TopClamp.position.y);
        _clampedPos.z = transform.position.z;

        transform.position = _clampedPos;
    }
}
