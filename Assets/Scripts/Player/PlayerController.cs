using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Move,
    Melee,
    Aim,
    Shoot,
    Death
}

public class PlayerController : MonoBehaviour
{

    public static PlayerController Singleton;
    
    //Components
    public PlayerAnimator Animator;
    public Rigidbody2D RgdBody;
    public PlayerStats PlayerStats;

    //Tools
    public Inventory Inventory;
    public Weapon Weapon;

    //Vars
    public float Speed = 10f;
    public PlayerState CurrentState;

    private Vector2 velocity;

    private void Awake()
    {
        Singleton = this;
    }

    private void OnEnable()
    {
        EventManager.Singleton.E_ItemCollected += CollectItem;
    }

    private void Start()
    {
        SetState(PlayerState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
    
    public void SetState(PlayerState state)
    {
        if (CurrentState == state)
            return;

        CurrentState = state;
        switch (state)
        {
            case PlayerState.Idle:
                Animator.SetAnimation(Animator.IdleAnim);
                break;
            case PlayerState.Move:
                Animator.SetAnimation(Animator.MoveAnim);
                break;
            case PlayerState.Melee:
                Animator.SetAnimation(Animator.AttackAnim);
                break;
        }
    }
 
    public void CollectItem(CollectableItem item,ActionType action)
    {
        switch (item.ItemType)
        {
            case ItemType.Deafult:
                break;
            case ItemType.Valueable:
                Inventory.AddValueableItem(item.GetComponent<ValueableItem>());
                break;
            case ItemType.Weapon:
                break;
            default:
                break;
        }
    }

    public void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        velocity.Set(horizontal, vertical);
        RgdBody.velocity = velocity * Speed * Time.deltaTime * 100;

        if (velocity.magnitude == 0)
        {
            SetState(PlayerState.Idle);
        }
        else
        {
            SetState(PlayerState.Move);
        }
    }

    public void MeleeAttack()
    {
        SetState(PlayerState.Melee);
    }


    public void FireWeapon()
    {
        Weapon.Fire();
    }

    public void GetHit(int damage)
    {
        PlayerStats.HP -= damage;

        if(PlayerStats.HP <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if(enemy == null)
        {
            return;
        }

        GetHit(enemy.Damage);
    }
}
