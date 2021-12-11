using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Kill,
    Buy,
    Sell,
    Collect,
}

public class EventManager : MonoBehaviour
{
    public static EventManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public delegate void D_CollectItem(CollectableItem item,ActionType action = ActionType.Collect);
    public event D_CollectItem E_ItemCollected;

    public delegate void D_ValuableItemAdded(ValueableItem item);
    public event D_ValuableItemAdded E_ValuableItemAdded;

    public delegate void D_EnemyDied(Enemy enemy,ActionType action = ActionType.Kill);
    public event D_EnemyDied E_EnemyDied;

    public delegate void D_LevelUp(int level = -1);
    public event D_LevelUp E_PlayerLevelUp;
    public void OnItemCollected(CollectableItem item)
    {
        E_ItemCollected?.Invoke(item);
    }

    public void OnItemCollected(CollectableItem item, ActionType action = ActionType.Collect)
    {
        E_ItemCollected?.Invoke(item, action);
    }

    public void OnEnemyDied(Enemy enemy, ActionType action = ActionType.Kill)
    {
        E_EnemyDied?.Invoke(enemy, action);
    }

    public void OnValuableItemAdded(ValueableItem item)
    {
        E_ValuableItemAdded?.Invoke(item);
    }

    public void OnPlayerLevelUp(int level = -1)
    {
        E_PlayerLevelUp?.Invoke(level);
    }


}
