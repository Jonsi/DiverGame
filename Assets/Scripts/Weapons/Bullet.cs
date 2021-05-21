using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Deafult,
    Bullet,
    Arrow
}
public class Bullet : MonoBehaviour
{
    public int Damage = 1;
    public BulletType BulletType = BulletType.Deafult;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
