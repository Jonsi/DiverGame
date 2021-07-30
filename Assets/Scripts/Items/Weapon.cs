using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Bullet BulletPrefab;
    public Transform BulletPoistion;
    public Transform BulletsHolder;
    public float BulletSpeed;

    public Vector2 _direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, Camera.main.transform.position.z));
        Vector2 bowPos = transform.position;
        _direction = mousePos - bowPos;

        transform.right = -_direction;
    }

    public void Fire()
    {
        Bullet bullet = Instantiate(BulletPrefab,BulletPoistion.position,transform.rotation);
        Rigidbody2D rgdBdy = bullet.GetComponent<Rigidbody2D>();
        rgdBdy.velocity = _direction.normalized * BulletSpeed;
    }

}
