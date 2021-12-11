using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{ 
    public void OnJumpComplete()
    {
        PlayerController.Singleton.SetState(PlayerState.IdleWater);
    }

    public void OnAimComplete()
    {
        PlayerController.Singleton.canShoot = true;
    }

    public void OnShootComplete()
    {
        PlayerController.Singleton.SetState(PlayerState.IdleWater);
    }

    public void OnFireComplete()
    {
        PlayerController.Singleton.SetState(PlayerState.IdleWater);
    }

    public void OnHitComplete()
    {
        PlayerController.Singleton.SetState(PlayerState.IdleWater);
    }

    public void OnDeathComplete()
    {
        PlayerController.Singleton.Die();
    }
}
