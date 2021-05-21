using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Patrol,
    Chase,
    Attack,
    Yield,
}
public class EnemyAI : MonoBehaviour
{
    public Rigidbody2D RgdBdy;
    public int Speed;
    public int ChaseSpeed;
    public int PatrolRadius = 5;
    public State State;

    public float rayDistance = 5f;
    public float ChaseDistance = 5f;
    public float PatrolFlipDistance = 0.2f;

    private PlayerController Player;
    private EnemyType EnemyType;

    public Vector2 SpawnPoint;
    public Vector2 PartolTarget;
    public Vector2 Target;


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
            case State.Yield:
                ReturnToSpawnPoint();
                break;
            default:
                break;
        }

    }

    public void SetState()
    {
        if (EnemyType != EnemyType.Agressive)
        {
            return;
        }

        float playerDistance = Vector2.Distance(transform.position, Player.transform.position);
        if (playerDistance <= ChaseDistance)
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
        Vector2 direction = Target - (Vector2)transform.position;
        RgdBdy.velocity = direction.normalized * speed * Time.fixedDeltaTime;
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
    }
    public void Charge()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Player.transform.position, rayDistance);

        if (hit.transform.gameObject.CompareTag("Player"))
        {

        }
    }

    void Flip()
    {
        Vector2 dir = Target - (Vector2)transform.position;
        transform.right = new Vector3(dir.x, transform.right.y, transform.right.z);
    }
}
