using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    NonAgressive,
    Agressive
}

public enum EnemyMovementType
{
    Walk,
    Swim
}
public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Death,
    Escape,
    Rest
}

public class Enemy : MonoBehaviour
{
    public EnemyType Type;
    public EnemyMovementType MovementType;
    public EnemyState CurrentState;
    public int HP = 1;
    public int Damage = 1;
    public float ExpPoints = 1;
}
