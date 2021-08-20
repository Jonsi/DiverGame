using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Singleton.E_EnemyDied += HandleEnemyDeath;
    }

    public int HP = 5;
    public int Mana = 5;
    public LevelSystem LevelSystem;

    public void HandleEnemyDeath(Enemy enemy,ActionType type)
    {
       LevelSystem.AddExp(enemy.ExpPoints);
    }

    private void OnDisable()
    {
        EventManager.Singleton.E_EnemyDied -= HandleEnemyDeath;
    }
}

[System.Serializable]
public class LevelSystem
{
    public int CurrentLevel = 1;
    public float CurrentExp = 0;
    public float ExpToNextLevel;
    [Range(1f, 2f)]
    public float NextLevelExpFactor = 1;


    public void AddExp(float amount)
    {
        CurrentExp += amount;

        if (CurrentExp >= ExpToNextLevel)
        {
            CurrentLevel++;
            CurrentExp -= ExpToNextLevel;
            ExpToNextLevel *= NextLevelExpFactor;
            EventManager.Singleton.OnPlayerLevelUp(CurrentLevel);
        }
    }
}