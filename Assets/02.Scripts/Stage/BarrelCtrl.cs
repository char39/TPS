using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이시 배럴 색상을 랜덤하게 변경
// 배럴이 5번 이상 총알에 맞았을 때 폭발 구현

public class BarrelCtrl : MonoBehaviour
{
    [SerializeField]private GameObject exploEffect;
    [SerializeField]private Texture[] textures;
    [SerializeField]private MeshRenderer meshRenderer;
    [SerializeField]private Rigidbody rb;
    [SerializeField]private int hitCount = 0;
    [SerializeField]private bool isExplo = false;
    [SerializeField]private AudioClip clip_explo;
    private readonly string bulletTag = "Bullet";
    private readonly string bulletTag_E = "E_Bullet";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        textures = Resources.LoadAll<Texture>("Texture");
        exploEffect = Resources.Load<GameObject>("Prefab/BigExplosionEffect");
        meshRenderer.material.mainTexture = textures[Random.Range(0, textures.Length - 1)];
    }

    /*  Projectile movement 총알 충돌시
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(bulletTag) || col.gameObject.CompareTag(bulletTag_E))
        {
            hitCount++;
            if (hitCount == 5)
            {
                ExplosionBarrel();
                StartCoroutine(GameManager.instance.CameraShake());
            }
        }
    }
    */
    
    void OnDamage(object[] paramsObj)
    {
        hitCount += (int)paramsObj[0];
        if (hitCount >= 5 && !isExplo)
        {
            isExplo = true;
            ExplosionBarrel();
            StartCoroutine(GameManager.instance.CameraShake());
        }
    }

    private void ExplosionBarrel()
    {
        GameObject eff = Instantiate(exploEffect, transform.position, Quaternion.identity);
        Destroy(eff, 2.0f);

        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, 1 << 7);
        // 배럴이 본인 위치에서 원형 반경 20f 만큼 (tag아님), layer7에 있는 배럴 레이어만 cols라는 배열에 담음.
        foreach (Collider col in cols)
        {
            Rigidbody rb_col = col.GetComponent<Rigidbody>();
            if (rb_col != null)
            {
                SoundManager.S_Instance.PlaySound(transform.position, clip_explo);
                rb_col.mass = 1.0f; // 배럴 무게를 기존 60.0f에서 1.0f로 바꿈
                rb_col.AddExplosionForce(1000, transform.position, 20f, 1200f);
                // 폭발력, 폭발 위치, 폭발 반경, 위로 솟구치는 힘
            }
            Invoke("BarrelMassChange", 3.0f);
        }
    }
    void BarrelMassChange()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, 1 << 7);
        // 배럴이 본인 위치에서 원형 반경 20f 만큼 (tag아님), layer7에 있는 배럴 레이어만 cols라는 배열에 담음.
        foreach (Collider col in cols)
        {
            Rigidbody rb_col = col.GetComponent<Rigidbody>();
            if (rb_col != null)
            {
                rb_col.mass = 60.0f; // 배럴 무게를 기존 60.0f으로 돌려줌
            }
        }
    }
    

}
