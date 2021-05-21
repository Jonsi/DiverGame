using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Singleton;
    public Inventory Inventory;
    public PlayerStats PlayerStats;
    public Weapon Weapon;

    private void Awake()
    {
        Singleton = this;
    }

    private void OnEnable()
    {
        EventManager.Singleton.E_ItemCollected += CollectItem;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireWeapon();
        }
    }

    public void CollectItem(CollectableItem item)
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
