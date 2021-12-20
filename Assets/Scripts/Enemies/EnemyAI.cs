using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Components")]
    public Enemy Enemy;
    public Rigidbody2D RgdBdy;
    public Animator Anmtr;

    [Header("General")]
    [Range(0f, 1f)] public float WaterResistance = 0.5f;

    [Header("Patrol")]
    [Range(0f, 1f)] public float PatrolSpeed = 5f;
    [Range(0f, 1f)] public float PatrolSpeedLerp = 0.5f;
    public float MinPatrolRadius = 2f;
    public float MaxPatrolRadius = 10f;
    [Range(0f, 1f)] public float PatrolClamp_Y = 0.5f;
    public float PatrolFlipDistance = 0.2f;
    public float ReachedTargetDistance = 2f;
    public float PatrolRestTimer = 2f;

    [Header("Escape")]
    public float EscapeDistance = 10f;
    [Range(0f, 1f)] public float EscapeClampY = 0.5f;
     public float EscapeSpeed = 10f;
    [Range(0f, 1f)] public float EscapeSpeedLerp = 0.5f;

    [Header("Agressive")]
    public float ChaseSpeed = 20;
    public float ChaseSpeedLerp = 20;
    public float ChaseDistance = 5f;
    public float AttackForce = 30;
    public float AttackDistance = 5f;
    public float attackDelay = 2f;

    private PlayerController _player;
    private float _playerDistance;
    private Vector2 _target;
    private float _speedLerp;
    private float _speed;
    private float _restTimer;

    private void Awake()
    {
        _player = PlayerController.Singleton;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetState(EnemyState.Idle);
    }
    public IEnumerator RestCOR()
    {
        SetState(EnemyState.Rest);
        yield return new WaitForSeconds(_restTimer);
        SetState(EnemyState.Idle);
    }

    private void Update()
    {

        if (Enemy.CurrentState == EnemyState.Rest)
        {
            return;
        }
        
        HandlePlayerDistance();
        HandleState();
    }

    private void FixedUpdate()
    {
        if(Enemy.CurrentState == EnemyState.Rest)
        {
            return;
        }

        MoveToTarget();
    }

    //State Management
    public void HandlePlayerDistance()
    {
        if(_player == null)
        {
            SetState(EnemyState.Patrol);
            return;
        }

        _playerDistance = Vector2.Distance(transform.position, _player.transform.position);

        if (Enemy.Type == EnemyType.Agressive)
        {
            if (_playerDistance < ChaseDistance)
            {
                if (_playerDistance < AttackDistance)
                {
                    SetState(EnemyState.Attack);
                    return;
                }

                SetState(EnemyState.Chase);
            }
            else
            {
                SetState(EnemyState.Patrol);
            }
        }
        else
        {
            if (_playerDistance < EscapeDistance)
            {
                SetState(EnemyState.Escape);
            }
            else
            {
                SetState(EnemyState.Patrol);
            }
        }
    }
    public void SetState(EnemyState state)
    {
        if (Enemy.CurrentState == state)
        {
            return;
        }

        switch (Enemy.CurrentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                StartPatrol();
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Death:
                break;
            case EnemyState.Escape:
                break;
            case EnemyState.Rest:
                break;
            default:
                break;
        }

        Enemy.CurrentState = state;
    }
    public void HandleState()
    {
        switch (Enemy.CurrentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                if(Vector2.Distance(transform.position,_target) < PatrolFlipDistance)
                {
                    _restTimer = PatrolRestTimer;
                    StartCoroutine("RestCOR");
                    return;
                }
                ChangeSpeed(PatrolSpeed, PatrolSpeedLerp);
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Death:
                break;
            case EnemyState.Escape:
                Escape();
                break;
            default:
                break;
        }
    }

    //State methods
    public void Death()
    {
        RgdBdy.velocity = Vector2.zero;
        Anmtr.Play("Death");
        EventManager.Singleton.OnEnemyDied(Enemy, ActionType.Kill);
    }
    private void StartPatrol()
    {
        Vector2 patrolDir = Random.insideUnitCircle.normalized * Random.Range(MinPatrolRadius, MaxPatrolRadius);
        patrolDir.y *= PatrolClamp_Y;
        SetTarget(patrolDir);
    }

    public void Chase()
    {
        ChangeSpeed(ChaseSpeed, ChaseSpeedLerp);
    }
    
    public void Escape()
    {
        if(_playerDistance > EscapeDistance)
        {
            SetState(EnemyState.Patrol);
            return;
        }

        ChangeSpeed(EscapeSpeed, EscapeSpeedLerp);
        Vector2 escapeDir = (_target - (Vector2)transform.position).normalized * -1;
        escapeDir.y *= EscapeClampY;
        SetTarget((Vector2)transform.position + escapeDir);
    }
    public void Attack()
    {
        RgdBdy.AddForce(RgdBdy.velocity * AttackForce);
        //animate
        //Rest
    }

    //AI
    public void SetTarget(Vector2 target)
    {
        if (Enemy.MovementType == EnemyMovementType.Walk)
        {
            _target.y = 0f;
        }

        _target = target;
    }
    public void MoveToTarget()
    {
        HandleFlip();
        RgdBdy.velocity = Vector2.Lerp(RgdBdy.velocity, (_target - (Vector2)transform.position), _speedLerp) *_speed;
        transform.right = Vector3.Lerp(transform.right,(Vector3) _target - transform.position, _speedLerp) * _speed;
        RgdBdy.AddForce(-RgdBdy.velocity * WaterResistance);
        Anmtr.speed = RgdBdy.velocity.sqrMagnitude;
    }
    void HandleFlip()
    {
        float Ydir = RgdBdy.velocity.x;
        Vector3 scale = Vector3.one;
        scale.x = -1;
        if (Ydir != 0)
            scale.y = Mathf.Abs(Ydir) / Ydir;

        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Projectile bullet = collision.gameObject.GetComponent<Projectile>();

        if (bullet == null)
        {
            return;
        }

        Enemy.HP -= bullet.Damage;
        if (Enemy.HP <= 0)
        {
            Death();
        }
    }
    private void ChangeSpeed(float speed, float speedLerp = 1f)
    {
        _speed = speed;
        _speedLerp = speedLerp;
    }
  
}