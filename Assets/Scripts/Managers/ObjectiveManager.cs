using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public List<Objective> Objectives;

    private void OnEnable()
    {
        EventManager.Singleton.E_EnemyDied += UpdateObjectives;
        EventManager.Singleton.E_ItemCollected += UpdateObjectives;
    }

    public void UpdateObjectives(Component component, ActionType action)
    {
        var otherName = component.gameObject.name;
        foreach (Objective objInList in Objectives)
        {
            var objName = objInList.Target.name;
            if (objInList.ActionType == action && otherName == objName)
            {
                objInList.Progress++;
            }
        }
    }

    private void OnDisable()
    {
        EventManager.Singleton.E_EnemyDied -= UpdateObjectives;
        EventManager.Singleton.E_ItemCollected -= UpdateObjectives;
    }

}
