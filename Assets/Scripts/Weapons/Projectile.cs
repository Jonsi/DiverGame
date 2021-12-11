using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Deafult,
    Bullet,
    Arrow
}

public class Projectile : MonoBehaviour
{
    public int Damage = 1;
    public ProjectileType ProjectileType = ProjectileType.Deafult;
    public Rigidbody2D rgdBdy;

    private Vector2 _direction;
    private float _speed;

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }

    private void FixedUpdate()
    {
        rgdBdy.velocity = _direction * _speed;
    }

}
