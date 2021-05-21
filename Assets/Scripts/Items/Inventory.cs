using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> Items;
    public int Coins;
    public int Gold;

    public void AddValueableItem(ValueableItem item)
    {
        switch (item.ValueType)
        {
            case ValueType.Deafult:
                break;
            case ValueType.Coins:
                Coins += item.Amount;
                break;
            case ValueType.Gold:
                Gold += item.Amount;
                break;
            default:
                break;
        }

        EventManager.Singleton.OnValuableItemAdded(item);
    }


}
