using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Default,
    Melee,
    Range
}

public class Weapon : CollectableItem
{
    public WeaponType Type;
    public int Damage = 1;

    public virtual void Fire()
    {

    }

}