using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject flarePrefab;
    public AudioClip audioClip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision col)
    {
        GameObject flare = Instantiate(flarePrefab, transform.position, Quaternion.identity);
        AudioSource audioSource = flare.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
        Destroy(flare, 1.0f);
        Destroy(gameObject);
    }
}
