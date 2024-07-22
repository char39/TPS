using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // public으로 선언된 클래스를 인스펙터 창에 노출
public class PlayerAnimation
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
    public AnimationClip sprint;
}

public class Player : MonoBehaviour
{
    #region vars 1

    public PlayerAnimation playerAnimation;
    private float moveSpeed = 5f;
    private float moveSpeedRun = 7.5f;
    private float finalMoveSpeed;
    private Vector3 moveDir;
    private float rotSpeed = 400f;
    private Rigidbody rb;
    private CapsuleCollider capcol;
    private Transform tr;
    private Animation ani;
    private float h, v, r;

    #endregion

    #region vars 2

    private bool isFire = false;
    private bool isReload = false;
    private int fireCount;
    private Transform firePos;
    private AudioSource source;
    [SerializeField]private AudioClip audioClip;
    public bool isRun;

    #endregion

    #region vars 3
    private ParticleSystem muzzFlash;
    private readonly string enemyTag = "Enemy";
    private readonly string enemyTag2 = "EnemySwat";
    private readonly string barrelTag = "Barrel";
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capcol = GetComponent<CapsuleCollider>();
        tr = GetComponent<Transform>();
        ani = GetComponent<Animation>();
        ani.Play(playerAnimation.idle.name);
        finalMoveSpeed = moveSpeed;

        firePos = GameObject.FindWithTag("FirePos").transform;
        source = GetComponent<AudioSource>();
        fireCount = 0;
        isRun = false;

        muzzFlash = firePos.GetChild(0).GetComponent<ParticleSystem>();
        muzzFlash.Stop();
    }

    void Update()
    {
        Debug.DrawRay(firePos.position, firePos.forward * 100f, Color.red);
        PlayerMove_All();
        StartCoroutine(PlayerGunFire());
    }

    private void PlayerMove_All()   // 플레이어의 움직이는 모든 것
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        r = Input.GetAxisRaw("Mouse X");
        moveDir = (h * Vector3.right + v * Vector3.forward).normalized;
        MoveAni();
        MoveRun();
        tr.Translate(moveDir * finalMoveSpeed * Time.deltaTime, Space.Self);     // Space.Self 로컬좌표, Space.World 절대좌표
        tr.Rotate(Vector3.up * r * rotSpeed * Time.deltaTime, Space.Self);
        
    }
    private void MoveRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1))
        {
            finalMoveSpeed = moveSpeedRun;
            isRun = true;
        }
        else
        {
            finalMoveSpeed = moveSpeed;
            isRun = false;
        }
    }
    private void MoveAni()  // 애니메이션만 재생
    {
        if (h > 0.1f)
        {
            ani.CrossFade(playerAnimation.runRight.name, 0.3f); // 0.3초 동안 천천히 전환
            if (finalMoveSpeed == moveSpeed)
                ani[playerAnimation.runRight.name].speed = 1.0f;
            else if (finalMoveSpeed == moveSpeedRun)
                ani[playerAnimation.runRight.name].speed = 1.5f;
        }
        else if (h < -0.1f)
        {
            ani.CrossFade(playerAnimation.runLeft.name, 0.3f);
            if (finalMoveSpeed == moveSpeed)
                ani[playerAnimation.runLeft.name].speed = 1.0f;
            else if (finalMoveSpeed == moveSpeedRun)
                ani[playerAnimation.runLeft.name].speed = 1.5f;
        }
        else if (v > 0.1f)
        {
            ani.CrossFade(playerAnimation.runForward.name, 0.3f);
            if (finalMoveSpeed == moveSpeed)
                ani[playerAnimation.runForward.name].speed = 1.0f;
            else if (finalMoveSpeed == moveSpeedRun)
                ani[playerAnimation.runForward.name].speed = 1.5f;
        }
        else if (v < -0.1f)
        {
            ani.CrossFade(playerAnimation.runBackward.name, 0.3f);
            if (finalMoveSpeed == moveSpeed)
                ani[playerAnimation.runBackward.name].speed = 1.0f;
            else if (finalMoveSpeed == moveSpeedRun)
                ani[playerAnimation.runBackward.name].speed = 1.5f;
        }
        else
        {
            ani.CrossFade(playerAnimation.idle.name, 0.3f);
            ani[playerAnimation.idle.name].speed = 1.0f;
        }
    }

    IEnumerator PlayerGunFire()
    {
        if (Input.GetMouseButtonUp(0) || !isFire || isRun)
        {
            muzzFlash.Stop();
        }
        if (Input.GetMouseButton(0) && !isFire && !isRun)
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
                isFire = true;
                muzzFlash.Play();
                LazerBeam.instance.PlayerLazerBeam();

                /*  1. 총알 Projectile movement 방식
                var bullets = ObjectPoolingManager_script.poolingManager.GetBulletPool();   // 비활성화 된 몇 번째 총알 반환
                if (bullets != null)    // 총알이 10개 다 활성화 되어있으면 작동X
                {
                    bullets.transform.position = firePos.position;
                    bullets.transform.rotation = firePos.rotation;
                    bullets.SetActive(true);
                    source.PlayOneShot(audioClip, 1.0f);
                    muzzFlash.Play();
                    yield return new WaitForSeconds(0.1f);
                }
                */
                RaycastHit hit; // 광선이 오브젝트에 충돌할 경우 충돌 지점이나 거리 등을 알려주는 광선 구조체
                if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f))    // 광선을 쐈을 때 반경 15f안에서 맞았는지 여부
                {
                    if (hit.collider.gameObject.tag == enemyTag || hit.collider.gameObject.tag == enemyTag2)
                    {
                        //Debug.Log("적 hit");
                        object[] paramsObj = new object[2];
                        paramsObj[0] = hit.point;       // 첫 번째 배열에 맞은 위치값을 전달
                        paramsObj[1] = 25f;             // 두 번째 배열에 총알 데미지값을 전달
                        hit.collider.gameObject.SendMessage("OnDamage", paramsObj, SendMessageOptions.DontRequireReceiver); // public이 아니어도 호출 가능
                    }
                    if (hit.collider.gameObject.tag == barrelTag)
                    {
                        //Debug.Log("배럴 hit");
                        object[] paramsObj = new object[3];
                        paramsObj[0] = 1;
                        paramsObj[1] = firePos.position;
                        paramsObj[2] = hit.point;
                        hit.collider.gameObject.SendMessage("OnDamage", paramsObj, SendMessageOptions.DontRequireReceiver);
                    }
                    if (hit.collider.gameObject)
                    {
                        //Debug.Log("벽, 바닥 hit");
                        object[] paramsObjs = new object[1];
                        paramsObjs[0] = hit.point;
                        hit.collider.gameObject.SendMessage("BulletHitEffect", paramsObjs, SendMessageOptions.DontRequireReceiver);
                    }
                }
                yield return new WaitForSeconds(0.1f);
                isFire = false;
            }
        }
        yield return null;
    }
}
