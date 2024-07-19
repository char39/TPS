using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    private float speed;
    public float damage;
    private Rigidbody rb;
    private TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        speed = 1000f;
        damage = 25f;
        rb = GetComponent<Rigidbody>();
        //gameObject.SetActive(false);
    }
    void BulletDisable()
    {
        this.gameObject.SetActive(false);
    }
    private void OnEnable() // 옵젝 켜질 때마다 실행
    {
        Invoke("BulletDisable", 3.0f);
        rb.AddForce(transform.forward * speed);
    }
    private void OnDisable()
    {
        trail.Clear();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rb.Sleep();
    }
}
