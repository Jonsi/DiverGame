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

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if(bullet == null)
        {
            return;
        }

        HP -= bullet.Damage;
    }
}
