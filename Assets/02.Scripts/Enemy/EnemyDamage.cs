using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private readonly string bulletTag = "Bullet";
    private GameObject bloodEffect;
    private float hp;

    void OnEnable()
    {
        hp = 100f;
        bloodEffect = Resources.Load<GameObject>("Effects/BulletImpactFleshBigEffects");
    }

    /* Projectile movement 총알 충돌감지
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(bulletTag))
        {
            ShowBloodEffect(col);
            hp -= col.gameObject.GetComponent<BulletMove>().damage;
            hp = Mathf.Clamp(hp, 0f, 100f);
            if (hp <= 0f)
                Die();
        }
    }
    */
    void Die()
    {
        GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
    } 

    private void ShowBloodEffect(Vector3 col) // blood 이펙트 보여주기
    {
        Vector3 pos = col;                  // 총알 맞은 위치를 할당
        Vector3 normal = col.normalized;    // 총알 맞은 방향을 할당
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, normal);
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 0.5f);
    }
    
    void OnDamage(object[] paramsObj)
    {
        ShowBloodEffect((Vector3)paramsObj[0]);
        hp -= (float)paramsObj[1];
        hp = Mathf.Clamp(hp, 0f, 100f);
        Debug.Log("" + hp);
        if (hp <= 0f)
            Die();
    }
}
