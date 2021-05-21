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
    public ItemType ItemType = ItemType.Deafult;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
