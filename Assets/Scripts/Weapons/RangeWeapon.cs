using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class RangeWeapon : Weapon
{
    [Header("Range")]
    public Projectile ProjectilePrefab;
    public float ShootForce = 2f;

    private void Start()
    {
        Type = WeaponType.Range;
    }

    public override void Fire()
    {
        InitProjectile();
    }

    private void InitProjectile()
    {
        Vector3 projectilePosition = PlayerController.Singleton.Spine.Bones.ProjectileBone.transform.position;
        Quaternion projectileRotation = PlayerController.Singleton.Spine.Bones.ProjectileBone.transform.rotation;
        Projectile pro = Instantiate(ProjectilePrefab, projectilePosition, projectileRotation) ;
        pro.SetDirection(PlayerController.Singleton.GetDirection());//todo: set direction from as bone direciton
        pro.SetSpeed(ShootForce);
        pro.StartCoroutine(pro.SelfDestroy(pro, pro.DestroyTimer));
    }
}