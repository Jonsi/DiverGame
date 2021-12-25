using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Deafult,
    Valueable,
    Weapon
}

[System.Serializable]
public class Item : MonoBehaviour
{
    [Header("Item")]
    public ItemType ItemType = ItemType.Deafult;
}
