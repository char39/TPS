using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject flarePrefab;
    public AudioClip clip_wall;
    public AudioClip clip_barrel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            SoundManager.Instance.PlaySound(transform.position, clip_wall);
            GameObject flare = Instantiate(flarePrefab, transform.position, Quaternion.identity);
            Destroy(flare, 1.0f);
        }
        if (col.gameObject.CompareTag("Barrel"))
        {
            SoundManager.Instance.PlaySound(transform.position, clip_wall);
        }
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("EnemySwat"))
        {
            
        }
        gameObject.SetActive(false);
    }
}
