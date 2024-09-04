using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyFire : MonoBehaviourPun
{
    [SerializeField] private AudioClip enemyFireClip;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTr;
    private EnemyAI E_AI;
    [SerializeField] private Transform enemyTr;
    [SerializeField] private Transform firePos;
    private readonly int hashFire = Animator.StringToHash("FireTrigger");   // Animator의 FireTrigger이란 이름의 Parameter
    private readonly int hashReload = Animator.StringToHash("ReloadTrigger");
    private float nextFire = 0.0f;              // 다음 시간에 발사할 시간 계산용 변수
    private readonly float fireRate = 0.1f;     // 총알 발사 간격
    private readonly float damping = 10.0f;     // 플레이어를 향하여 회전하는 속도 보간수치
    public bool isFire = false;
    
    [Header("Reload")]
    private readonly float reloadTime = 1.8f;
    private readonly int maxBullet = 10;
    [SerializeField] private int curBullet = 0;
    private bool isReload = false;
    private WaitForSeconds reloadws;
    private AudioClip reloadClip;
    public MeshRenderer muzzFlash;


    void Start()
    {
        E_AI = GetComponent<EnemyAI>();

        enemyFireClip = Resources.Load<AudioClip>("Audio/p_m4_1");
        animator = GetComponent<Animator>();
        if (E_AI != null)
            playerTr = E_AI.playerTr;
        enemyTr = GetComponent<Transform>();
        firePos = transform.GetChild(4).GetChild(0).GetChild(0).transform;
        curBullet = maxBullet;
        reloadws = new WaitForSeconds(reloadTime);
        reloadClip = Resources.Load<AudioClip>("Audio/p_reload");
        muzzFlash = firePos.GetChild(0).GetComponent<MeshRenderer>();
        muzzFlash.enabled = false;
    }

    void Update()
    {
        if (E_AI != null)
            playerTr = E_AI.playerTr;
        if (isFire && !GameManager.instance.isGameOver)
        {
            if (playerTr != null)
            {
                if (Time.time >= nextFire && PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("Fire_F", RpcTarget.All);
                    nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f);
                }
                Vector3 playerLooknormal = playerTr.position - enemyTr.position;
                Quaternion rot = Quaternion.LookRotation(playerLooknormal);
                enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, damping * Time.deltaTime);
            }
        }
    }
    [PunRPC]
    private void Fire_F()
    {
        StartCoroutine(Fire());
    }
    IEnumerator Fire()      // 재장전, 총알 발사 메서드
    {
        if (isReload)
        {
            isReload = false;
            StartCoroutine(Reload());
        }
        else if (curBullet > 0 && !isReload)
        {
            isReload = --curBullet % maxBullet == 0;

            PhotonNetwork.InstantiateRoomObject("E_Bullet", firePos.position, firePos.rotation);

            animator.SetTrigger(hashFire);
            //SoundManager.Instance.PlaySound(firePos.position, enemyFireClip);
            StartCoroutine(ShowMuzzleFlash());
            yield return new WaitForSeconds(0.1f);

        }
    }
    IEnumerator Reload()
    {
        animator.SetTrigger(hashReload);
        //SoundManager.Instance.PlaySound(transform.position, reloadClip);
        yield return reloadws;
        curBullet = maxBullet;
    }
    IEnumerator ShowMuzzleFlash()
    {
        muzzFlash.enabled = true;

        //Vector2 offset = new Vector2(Random.Range(1f, 2f), Random.Range(1f, 2f)) * 0.5f;
        //muzzFlash.transform.localScale = Vector3.one * offset;

        muzzFlash.transform.localScale = Vector3.one * Random.Range(0.8f, 1.5f);
        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        muzzFlash.transform.localRotation = rot;

        yield return new WaitForSeconds(0.02f);
        muzzFlash.enabled = false;
    }

}

