using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int Coins;
    public int Gold;
    public RangeWeapon RangeWeapon;
    public Weapon MeleeWeapon;
    public List<InventoryItem> CollectableItems;

    public void AddValueableItem(ValueableItem item)
    {
        switch (item.ValueType)
        {
            case ValueType.Deafult:
                break;
            case ValueType.Coins:
                Coins++;
                break;
            case ValueType.Gold:
                Gold++;
                break;
            default:
                break;
        }

        EventManager.Singleton.OnValuableItemAdded(item);
    }
}

[System.Serializable]
public class InventoryItem
{
    public CollectableItem Item;
    public int count = 1;
}