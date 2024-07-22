using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHit : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject flarePrefab;
    public AudioClip clip_wall;
    public AudioClip clip_barrel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        flarePrefab = Resources.Load<GameObject>("Prefab/FlareMobile");

    }

    void BulletHitEffect(object[] paramsObj)
    {
        //SoundManager.S_Instance.PlaySound((Vector3)paramsObj[0], clip_wall);
        GameObject flare = Instantiate(flarePrefab, (Vector3)paramsObj[0], Quaternion.identity);
        Destroy(flare, 1.0f);
    }
}
