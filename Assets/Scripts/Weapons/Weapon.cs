using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Default,
    Melee,
    Range
}

public class Weapon : AttachableItem
{
    [Header("Weapon")]
    public WeaponType Type;

    public virtual void Fire()
    {

    }

}