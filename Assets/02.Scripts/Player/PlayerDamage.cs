using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    private readonly string bullet_e_Tag = "E_Bullet";
    private GameObject bloodEffect;
    public int hp = 0;
    public int maxHp = 100;
    private bool isDie = false;

    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("Effects/BulletImpactFleshBigEffects");
        hp = maxHp;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(bullet_e_Tag))
        {
            ShowBloodEffect(col);
            hp -= 5;
            if (hp <= 0 && !isDie)
                PlayerDie(col);

        }
    }

    public void PlayerDie(Collision col)
    {
        isDie = true;
        GameManager.instance.isGameOver = true;
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemy.Length; i++)
            enemy[i].gameObject.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        GameObject[] enemySwat = GameObject.FindGameObjectsWithTag("EnemySwat");
        for (int i = 0; i < enemySwat.Length; i++)
            enemySwat[i].gameObject.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
    }

    private void ShowBloodEffect(Collision col)
    {
        Vector3 pos = col.contacts[0].point;        // 총알 맞은 위치를 할당
        Vector3 normal = col.contacts[0].normal;    // 총알 맞은 방향을 할당
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 0.5f);
    }

}
