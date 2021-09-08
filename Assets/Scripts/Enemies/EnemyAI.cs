using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Patrol,
    Chase,
    Attack,
    Death,
    Yield,
}
public class EnemyAI : MonoBehaviour
{
    public Enemy Enemy;
    public Rigidbody2D RgdBdy;
    public Animator Anmtr;
    public int Speed;
    public int ChaseSpeed;
    public float ChargeForce;
    public int PatrolRadius = 5;
    public State State;

    public float ChargeDistance = 5f;
    public float ChaseDistance = 5f;
    public float PatrolFlipDistance = 0.2f;

    public Vector2 SpawnPoint;
    public Vector2 PartolTarget;
    public Vector2 Target;

    private PlayerController Player;
    private EnemyType EnemyType;
    private Vector2 _direction;
    private float _playerDistance;


    // Start is called before the first frame update
    void Start()
    {
        EnemyType = gameObject.GetComponent<Enemy>().EnemyType;
        State = State.Patrol;
        Player = PlayerController.Singleton;

        SpawnPoint = transform.position;
        PartolTarget = new Vector2(SpawnPoint.x + PatrolRadius ,SpawnPoint.y);
        SetTarget(PartolTarget);
    }

    // Update is called once per frame
    void Update()
    {
        SetState();

        switch (State)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                break;
            case State.Death:
                break;
            case State.Yield:
                ReturnToSpawnPoint();
                break;
            default:
                break;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if (bullet == null)
        {
            return;
        }

        Enemy.HP -= bullet.Damage;
        if(Enemy.HP <= 0)
        {
            RgdBdy.velocity = Vector2.zero;
            State = State.Death;
            Anmtr.SetTrigger("death");
        }
    }

    public void Die()
    {
        EventManager.Singleton.OnEnemyDied(Enemy, ActionType.Kill);
        Destroy(gameObject);
    }
    public void SetState()
    {
        if (EnemyType != EnemyType.Agressive || State == State.Death)
        {
            return;
        }

        _playerDistance = Vector2.Distance(transform.position, Player.transform.position);
        if (_playerDistance <= ChaseDistance)
        {
            SetTarget(Player.transform.position);
            State = State.Chase;
        }
        else
        {
            SetTarget(PartolTarget);
            State = State.Patrol;
        }
    }

    public void Patrol()
    {
        MoveToTarget(Speed);

        if(Vector2.Distance(transform.position,Target) <= PatrolFlipDistance)
        {
            PatrolRadius *= -1;
            PartolTarget = new Vector2(SpawnPoint.x + PatrolRadius, SpawnPoint.y);
            SetTarget(PartolTarget);
        }
    }

    public void MoveToTarget(float speed)
    {
        Flip();
        _direction = Target - (Vector2)transform.position;
        RgdBdy.velocity = _direction.normalized * speed * Time.fixedDeltaTime;
    }

    public void SetTarget(Vector2 target)
    {
        Target = target;
    }


    public void ReturnToSpawnPoint()
    {

    }

    public void Chase()
    {
        MoveToTarget(ChaseSpeed);
        Charge();
    }
    public void Charge()
    {
        if(_playerDistance < ChargeDistance)
        {
            Anmtr.SetTrigger("attack");
            RgdBdy.AddForce(_direction * ChargeForce);
        }
    }

    void Flip()
    {
        Vector2 dir = Target - (Vector2)transform.position;
        transform.right = new Vector3(dir.x, transform.right.y, transform.right.z);
    }
}
