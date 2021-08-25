using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Default,
    Normal,
    Agressive
}

public class Enemy : MonoBehaviour
{
    public EnemyType EnemyType;
    public int HP = 1;
    public int Damage = 1;
    public float ExpPoints = 1;

}
