using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public delegate void D_CollectItem(CollectableItem item);
    public event D_CollectItem E_ItemCollected;

    public delegate void D_ValuableItemAdded(ValueableItem item);
    public event D_ValuableItemAdded E_ValuableItemAdded;

    public void OnItemCollected(CollectableItem item)
    {
        E_ItemCollected?.Invoke(item);
    }

    public void OnValuableItemAdded(ValueableItem item)
    {
        E_ValuableItemAdded?.Invoke(item);
    }
}
