using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    private readonly string bullet_e_Tag = "E_Bullet";
    private GameObject bloodEffect;

    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("Effects/BulletImpactFleshBigEffects");
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(bullet_e_Tag))
        {
            Vector3 pos = col.contacts[0].point;        // 총알 맞은 위치를 할당
            Vector3 normal = col.contacts[0].normal;    // 총알 맞은 방향을 할당
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
            GameObject blood = Instantiate(bloodEffect, pos, rot);
            Destroy(blood, 0.5f);
        }
    }
}
