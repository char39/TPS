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
            SoundManager.S_Instance.PlaySound(transform.position, clip_wall);
            GameObject flare = Instantiate(flarePrefab, transform.position, Quaternion.identity);
            Destroy(flare, 1.0f);
        }
        if (col.gameObject.CompareTag("Barrel"))
        {
            SoundManager.S_Instance.PlaySound(transform.position, clip_wall);
        }
        gameObject.SetActive(false);
    }
}
