using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public enum PlayerSkin
{
    DefaultSkin,
    stand_blue,
    stand_purple,
    swim_blue,
    swim_purple
}
public enum PlayerState
{
    none,
    IdleBoat,
    JumpFromBoat,
    IdleWater,
    SwimMelee,
    SwimGun,
    Aim,
    Shoot,
    Hit,
    Death
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController Singleton;

    [Header("General")]
    public Transform JumpPos;
    public float Speed = 10f;
    [Range(0f, 1f)] public float SpeedLerp = 0.5f;
    public PlayerSkin StandSkin = PlayerSkin.stand_blue;
    public PlayerSkin SwimSkin = PlayerSkin.swim_blue;
    public PlayerState State;
    public bool UpdateState = false;

    [Header("Animation")]
    public Transform JumpFixPos;
    [Range(0f, 1f)] public float JumpSpeed = 1;
    public float AimSpeed = 1;
    [Range(0f, 1f)] public float TimeScaleClamp = 0.2f;

    [Header("Tools")]
    public Inventory Inventory;
    public Weapon Weapon;

    [Header("Components")]
    public Rigidbody2D RgdBody;
    public PlayerStats PlayerStats;
    public PlayerSpine Spine;

    //PRIVATE
    private PlayerState _state = PlayerState.IdleBoat;
    private Vector2 _direction;
    private Vector2 _velocity;
    private bool _canMove = false;
    private bool _isAiming = false;
    public bool canShoot = false;

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
        SetState(State, true);
        Weapon = Inventory.RangeWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdateState)
        {
            SetState(State, true);
            UpdateState = false;
        }

        Rotate();
        Flip();
        HandleInput();
        HandleState();
    }
    private void FixedUpdate()
    {
        Move();
    }

    public void SetState(PlayerState state,bool force = false)
    {
        if (!force && _state == state)
            return;

        State = state;
        _state = state;

        switch (_state)
        {
            case PlayerState.IdleBoat:
                Spine.Bones.SwimRootBone.overrideAlpha = 0;
                _canMove = false;
                transform.position = JumpPos.position;
                Spine.SetAnimation(0,Spine.IdleOnBoatAnim, StandSkin,true);
                break;
            case PlayerState.JumpFromBoat:
                transform.position =JumpFixPos.position;
                _canMove = false;
                Spine.SetAnimation(0,Spine.JumpFromBoatAnim, SwimSkin,false,JumpSpeed);
                break;
            case PlayerState.IdleWater:
                _isAiming = false;
                _canMove = true;
                Spine.SetAnimation(0,Spine.IdleInWaterAnim, SwimSkin,true);
                Spine.Bones.SwimRootBone.overrideAlpha = 1;
                break;
            case PlayerState.SwimMelee:
                Spine.SetAnimation(0,Spine.SwimMeleeAnim,SwimSkin,true);
                break;
            case PlayerState.SwimGun:
                Spine.SetAnimation(0,Spine.SwimGunAnim, SwimSkin,true);
                break;
            case PlayerState.Aim:
                _velocity = Vector2.zero;
                _isAiming = true;
                _canMove = false;
                Spine.SetAnimation(0, Spine.SwimMeleeAnim, SwimSkin, true, 0.5f);
                Spine.SetAnimation(1,Spine.AimAnim, SwimSkin,false,AimSpeed);
                break;
            case PlayerState.Shoot:
                break;
            case PlayerState.Hit:
                Spine.SetAnimation(0, Spine.AimAnim, SwimSkin, false, AimSpeed);
                break;
            case PlayerState.Death:
                break;
            default:
                break;
        }
    }
    public void HandleState()
    {
        switch (_state)
        {
            case PlayerState.none:
                break;
            case PlayerState.IdleBoat:
                break;
            case PlayerState.JumpFromBoat:
                //HandleJump();
                break;
            case PlayerState.IdleWater:
                break;
            case PlayerState.SwimMelee:
            case PlayerState.SwimGun:
                Spine.SetSpeed(Mathf.Lerp(RgdBody.velocity.magnitude, _velocity.magnitude, SpeedLerp) * TimeScaleClamp);
                //Handle swim rotaion
                break;
            case PlayerState.Aim:
                _direction = GetMouseDirection();
                //Rotate();
                break;
            case PlayerState.Shoot:
                if (canShoot)
                {
                    Spine.SetAnimation(1, Spine.FireAnim, SwimSkin, false);
                    Weapon.Fire();
                    canShoot = false;
                }
                break;
            case PlayerState.Death:
                break;
            default:
                break;
        }
    }
    public void HandleInput()
    {
        if (_canMove)
        {
            GetMovementInput();
            SetMovementState();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetState(PlayerState.JumpFromBoat);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && !_isAiming)
        {
            SetState(PlayerState.Aim);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            SetState(PlayerState.Shoot);
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
    public void SetMovementState()
    {
        if (_velocity.sqrMagnitude == 0)
        {
            SetState(PlayerState.IdleWater);
        }
        else
        {
            SetState(PlayerState.SwimMelee);
        }
    }
    private void GetMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        _velocity.Set(horizontal, vertical);
        _velocity *= Speed;

        if(_velocity.sqrMagnitude > Mathf.Epsilon)
        {
            _direction.Set(horizontal, vertical);
        }
    }
    private void Flip()
    {
        if (_direction.x != 0)
        {
            var scaleX = Mathf.Abs(_direction.x) / _direction.x;
            Spine.Bones.SwimRootBone.transform.localScale = new Vector3(1,scaleX, 1);
        }
    }
    public void Move()
    {
        RgdBody.velocity = Vector2.Lerp(RgdBody.velocity, _velocity, SpeedLerp);
    }
    public void MeleeAttack()
    {
        SetState(PlayerState.SwimGun);
    }
    public void Rotate()
    {
        float rot_z = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Spine.Bones.SwimRootBone.transform.rotation = Quaternion.AngleAxis(rot_z, Vector3.forward);
    }
    public void GetHit(int damage)
    {
        PlayerStats.HP -= damage;

        if(PlayerStats.HP <= 0)
        {
            SetState(PlayerState.Death);
        }
        else
        {
            SetState(PlayerState.Hit);
        }

    }
    public void Die()
    {
        Destroy(gameObject);
    }

    public Vector2 GetDirection()
    {
        return _direction.normalized;
    }
    public Vector3 GetMouseDirection()
    {
        return (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
    }
    public void ToggleWeapon()
    {
        if(Weapon.Type == WeaponType.Range)
        {
            Weapon = Inventory.MeleeWeapon;
        }
        else
        {
            Weapon = Inventory.RangeWeapon;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if(enemy == null || enemy.Type != EnemyType.Agressive)
        {
            return;
        }

        GetHit(enemy.Damage);
    }


    /*private void HandleJump()//REMOVE?
    {
        _timer += Time.deltaTime;
        float frame = _timer / JumpTime;
        float xPos = JumpPos.position.x + frame * (JumpLandPos.position.x - JumpPos.position.x);
        float yPos = JumpPos.position.y + (JumpCurveY.Evaluate(frame));
        //transform.position = new Vector2(xPos, yPos);
    }
    
    public void SetJumpCurve()
    {
    Keyframe Topkeyframe = new Keyframe(SpeedCurve.keys[1].time, JumpTopPos.position.y - transform.position.y);
    Keyframe BottomKeyframe = new Keyframe(SpeedCurve.keys[2].time, JumpLandPos.position.y - transform.position.y);
    JumpCurveY.MoveKey(1, Topkeyframe);
    JumpCurveY.MoveKey(2, BottomKeyframe);
    }*/
}