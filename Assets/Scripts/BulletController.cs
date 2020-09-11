using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OffScreenWrapper))]
public class BulletController : MonoBehaviour
{

    public float bulletSpeed = 10f;

    public GameObject bulletAnchor;

    private void Start()
    {
        // check if bullet anchor exists, if not create it or assign it as found value
        bulletAnchor = GameObject.Find("BulletAnchor") == null ?
            new GameObject("BulletAnchor") :
            GameObject.Find("BulletAnchor");

        // make the bullet a child of the bulletanchor GO
        transform.parent = bulletAnchor.transform;

        // Destroy the bullet after 2 seconds
        Invoke("DestroyMe", 2f);

        MoveBullet();
    }

    private void MoveBullet()
    {
        // this sets the velocity of the bullet - rigidbody is used vs transform.translate bc it needs to bounch off of things (transform/translate won't allow that by itself)
        GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

}
