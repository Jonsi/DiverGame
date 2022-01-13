using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public Boundriies WorldBoundries;

    void Update()
    {/*
        _clampedPos = PlayerController.Singleton.transform.position;
        //_clampedPos.x = Mathf.Clamp(transform.position.x, LeftClamp.position.x, RightClamp.position.x);
        _clampedPos.y = Mathf.Clamp(PlayerController.Singleton.transform.position.y, WorldBoundries.BottomClamp.position.y, WorldBoundries.TopClamp.position.y);
        _clampedPos.z = transform.position.z;*/

        transform.position = WorldBoundries.ClampPosToBounderies(PlayerController.Singleton.transform.position);
    }
}
