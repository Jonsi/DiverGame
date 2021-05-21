using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ValueType
{
    Deafult,
    Coins,
    Gold
}

public class ValueableItem : CollectableItem
{
    public ValueType ValueType = ValueType.Deafult;
    
}
