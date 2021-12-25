using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Deafult,
    Bullet,
    Projectile
}

public class Projectile : AttachableItem
{
    public int Damage = 1;
    public float DestroyTimer = 5f;
    public ProjectileType ProjectileType = ProjectileType.Deafult;
    public Rigidbody2D rgdBdy;

    private Vector2 _direction;
    private float _speed;
    private bool _hasHit = false;

    public IEnumerator SelfDestroy(Projectile obj, float time)
    {
        yield return new WaitForSeconds(time);

        if (!_hasHit)
        {
            Destroy(obj.gameObject);
        }
    }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.gameObject.GetComponent<Enemy>();

        if(enemy!= null)
        {
            _hasHit = true;
            StickTo(enemy.transform);
        }
    }

    private void StickTo(Transform objTransform)
    {
        rgdBdy.velocity = Vector2.zero;
        rgdBdy.isKinematic = true;
        transform.parent = objTransform;
    }
}
