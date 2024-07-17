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

    //[SerializeField]private GameObject bulletPrefab;
    private bool isFire = false;
    private Transform firePos;
    private AudioSource source;
    [SerializeField]private AudioClip audioClip;

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
    }

    void Update()
    {
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
            finalMoveSpeed = moveSpeedRun;
        else
            finalMoveSpeed = moveSpeed;
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
        if (Input.GetMouseButton(0) && !isFire)
        {
            //Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            // 오브젝트 풀링 방식을 아래에 작성함.
            isFire = true;
            var bullets = ObjectPoolingManager_script.poolingManager.GetBulletPool();   // 비활성화 된 몇 번째 총알 반환
            if (bullets != null)    // 총알이 10개 다 활성화 되어있으면 작동X
            {
                bullets.transform.position = firePos.position;
                bullets.transform.rotation = firePos.rotation;
                bullets.SetActive(true);
                source.PlayOneShot(audioClip, 1.0f);
                yield return new WaitForSeconds(0.1f);
            }
            isFire = false;
        }
        yield return null;
    }
}
