using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : Item
{
    public int Amount = 1;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if(player == null)
        {
            return;
        }

        EventManager.Singleton.OnItemCollected(this);
        Destroy(gameObject);
    }

}
