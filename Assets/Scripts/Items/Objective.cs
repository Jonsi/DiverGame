using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum ObjectiveStatus
{
    Default,
    Locked,
    InProgress,
    Complete
}

[System.Serializable]
public class Objective
{
    public ObjectiveStatus Status;
    public ActionType ActionType;
    public GameObject Target;
    public int GoalCount = 1;
    public int Progress;
    public List<Reward> Rewards;
}

[System.Serializable]
public class Reward
{
    public Item Item;
    public int count;
}
