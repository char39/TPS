using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [SerializeField] private AudioClip enemyFireClip;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform enemyTr;
    [SerializeField] private Transform firePos;
    private readonly int hashFire = Animator.StringToHash("FireTrigger");   // Animator의 FireTrigger이란 이름의 Parameter
    private float nextFire = 0.0f;              // 다음 시간에 발사할 시간 계산용 변수
    private readonly float fireRate = 0.1f;     // 총알 발사 간격
    private readonly float damping = 10.0f;     // 플레이어를 향하여 회전하는 속도 보간수치
    public bool isFire = false;
    public bool isReload = false;
    private int fireCount = 0;
    
    void Start()
    {
        enemyFireClip = Resources.Load<AudioClip>("Audio/p_m4_1");
        animator = GetComponent<Animator>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        enemyTr = GetComponent<Transform>();
        firePos = transform.GetChild(3).GetChild(0).GetChild(0).transform;
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
            Vector3 playerLooknormal = playerTr.position - enemyTr.position;
            Quaternion rot = Quaternion.LookRotation(playerLooknormal);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, damping * Time.deltaTime);
        }    
    }
    
    IEnumerator Fire()
    {
        if (fireCount == 10 && !isReload)
        {
            isReload = true;
            yield return new WaitForSeconds(1.0f);
            fireCount = 0;
            isReload = false;
        }
        else if (fireCount < 10 && !isReload)
        {
            fireCount++;
            var bullets = ObjectPoolingManager_script.poolingManager.GetE_BulletPool();   // 비활성화 된 몇 번째 총알 반환
            if (bullets != null)    // 총알이 10개 다 활성화 되어있으면 작동X
            {
                bullets.transform.position = firePos.position;
                bullets.transform.rotation = firePos.rotation;
                bullets.SetActive(true);
                animator.SetTrigger(hashFire);
                SoundManager.S_Instance.PlaySound(firePos.position, enemyFireClip);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
