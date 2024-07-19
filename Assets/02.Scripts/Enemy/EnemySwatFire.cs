using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwatFire : MonoBehaviour
{
    [SerializeField] private AudioClip enemySwatFireClip;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform enemySwatTr;
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
        enemySwatFireClip = Resources.Load<AudioClip>("Audio/p_m4_1");
        animator = GetComponent<Animator>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        enemySwatTr = GetComponent<Transform>();
        firePos = transform.GetChild(2).GetChild(0).GetChild(0).transform;
        curBullet = maxBullet;
        reloadws = new WaitForSeconds(reloadTime);
        reloadClip = Resources.Load<AudioClip>("Audio/p_reload");
        muzzFlash = firePos.GetChild(0).GetComponent<MeshRenderer>();
        muzzFlash.enabled = false;
    }

    void Update()
    {
        if (isFire)
        {
            if (Time.time >= nextFire)
            {
                StartCoroutine(Fire());
                nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f);
            }
            Vector3 playerLooknormal = playerTr.position - enemySwatTr.position;
            Quaternion rot = Quaternion.LookRotation(playerLooknormal);
            enemySwatTr.rotation = Quaternion.Slerp(enemySwatTr.rotation, rot, damping * Time.deltaTime);
        }    
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
            var bullets = ObjectPoolingManager_script.poolingManager.GetE_BulletPool();   // 비활성화 된 몇 번째 총알 반환
            if (bullets != null)    // 총알이 다 활성화 되어있으면 작동X
            {
                bullets.transform.position = firePos.position;
                bullets.transform.rotation = firePos.rotation;
                bullets.SetActive(true);
                animator.SetTrigger(hashFire);
                SoundManager.S_Instance.PlaySound(firePos.position, enemySwatFireClip);
                StartCoroutine(ShowMuzzleFlash());
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    IEnumerator Reload()
    {
        animator.SetTrigger(hashReload);
        SoundManager.S_Instance.PlaySound(transform.position, reloadClip);
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

