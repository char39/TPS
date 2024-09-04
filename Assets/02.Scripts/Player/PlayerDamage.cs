using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataInfo;
using Photon.Pun;

public class PlayerDamage : MonoBehaviourPun
{
    private readonly string bullet_e_Tag = "E_Bullet";
    private GameObject bloodEffect;
    private Image bloodScreen;
    public int current_hp = 0;
    public int Initia_Hp = 100;
    private bool isDie = false;

    public delegate void PlayerDie_Handler();               // 델리게이트
    public static event PlayerDie_Handler OnPlayerDie;      // 델리게이트 이벤트

    private Image image_current_hp;

    void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;    // 함수를 이벤트로 등록
    }

    void UpdateSetup()
    {
        current_hp += (int)GameManager.instance.gameData.hp - Initia_Hp;
        Initia_Hp = (int)GameManager.instance.gameData.hp;  // 게임매니저에서 최대hp가 바뀌면 실시간으로 최대hp를 변경해줌
    }

    void Start()
    {
        Initia_Hp = (int)GameManager.instance.gameData.hp;
        current_hp = Initia_Hp;

        bloodEffect = Resources.Load<GameObject>("Effects/BulletImpactFleshBigEffects");
        bloodScreen = GameObject.Find("Canvas_UI").transform.GetChild(0).GetComponent<Image>();
        bloodScreen.color = Color.clear;
        image_current_hp = GameObject.Find("Canvas_UI").transform.GetChild(2).GetChild(2).GetComponent<Image>();
        UpdateHp();
    }

    /*     void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.CompareTag(bullet_e_Tag))
            {
                ShowBloodEffect(col);
                current_hp -= 1;
                UpdateHp();
                if (current_hp <= 0 && !isDie)
                    PlayerDie(col);
                StartCoroutine(ShowBloodScreen());
            }
        } */

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(bullet_e_Tag))
        {
            float damage = 2;
            Vector3 hitPoint = col.contacts[0].point;

            photonView.RPC("TakeDamage", RpcTarget.All, damage, hitPoint);
        }
    }

    [PunRPC]
    void TakeDamage(float damage, Vector3 hitPoint)
    {
        if (photonView.IsMine)
            TakeDamageLocal(damage, hitPoint);
    }
    
    void TakeDamageLocal(float damage, Vector3 hitPoint)
    {
        ShowBloodEffect(hitPoint);
        current_hp -= (int)damage;
        UpdateHp();
        if (current_hp <= 0 && !isDie)
            PlayerDie(hitPoint);
        StartCoroutine(ShowBloodScreen());
    }

    public void PlayerDie(Vector3 hitpoint)
    {
        OnPlayerDie();
    }

    private void ShowBloodEffect(Vector3 hitpoint)
    {
        Vector3 pos = hitpoint;
        Vector3 normal = hitpoint.normalized;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 0.5f);
    }
    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1f, 0f, 0f, Random.Range(0.25f, 0.35f));
        yield return new WaitForSeconds(0.1f);
        bloodScreen.color = Color.clear;    // 색상값을 전부 (R,G,B,Alpha) 0, 0, 0, 0 으로 변경
    }

    void UpdateHp()
    {
        //current_hp = Mathf.Clamp(current_hp, 0, 100);
        image_current_hp.fillAmount = (float)current_hp / (float)Initia_Hp;
        if (image_current_hp.fillAmount <= 0.2f)
            image_current_hp.color = Color.red;
        else if (image_current_hp.fillAmount <= 0.5f)
            image_current_hp.color = Color.yellow;
        else if (image_current_hp.fillAmount <= 1f)
            image_current_hp.color = Color.green;
    }

}
