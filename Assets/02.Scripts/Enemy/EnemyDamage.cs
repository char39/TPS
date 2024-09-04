using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviourPun, IPunObservable
{
    private readonly string bulletTag = "Bullet";
    private GameObject bloodEffect;
    private float hp;
    private float maxHp = 100f;

    public Image hpBar;
    public Text hpText;

    void OnEnable()
    {
        hp = maxHp;
        UpdateHpUI();
        bloodEffect = Resources.Load<GameObject>("Effects/BulletImpactFleshBigEffects");
    }

    void Update()
    {
        UpdateHpUI();
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
    [PunRPC]
    void Die()
    {
        GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
    }
    void ExplosionDie()
    {
        hp = 0f;
        GetComponent<EnemyAI>().state = EnemyAI.State.EXPLOSIONDIE;
    }

    private void ShowBloodEffect(Vector3 col) // blood 이펙트 보여주기
    {
        Vector3 pos = col;                  // 총알 맞은 위치를 할당
        Vector3 normal = col.normalized;    // 총알 맞은 방향을 할당
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, normal);
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 0.5f);
    }
    
    public void OnDamage(object[] paramsObj)
    {
        photonView.RPC("OnDamage_F", RpcTarget.All, paramsObj);
    }

    [PunRPC]
    void OnDamage_F(Vector3 hitPosition, float damage)
    {
        ShowBloodEffect(hitPosition);
        hp -= damage;
        hp = Mathf.Clamp(hp, 0f, 100f);
        if (hp <= 0f)
            photonView.RPC("Die", RpcTarget.All);
    }

    void UpdateHpUI()
    {
        hpBar.fillAmount = hp / maxHp;
        hpText.text = $"HP : <color=#FFAAAA>{hp}</color>";
        if (hpBar.fillAmount < 0.2f)
            hpBar.color = Color.red;
        else if (hpBar.fillAmount < 0.5f)
            hpBar.color = Color.yellow;
        else
            hpBar.color = Color.green;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
        }
        else
        {
            hp = (float)stream.ReceiveNext();
        }
    }
}
