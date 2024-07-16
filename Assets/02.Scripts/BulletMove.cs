using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    private float speed;
    private Rigidbody rb;
    
    void Start()
    {
        speed = 1000f;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed);
        Destroy(gameObject, 5f);
    }
}
