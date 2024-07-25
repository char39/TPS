using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class EnemyAI : MonoBehaviour
{
    public enum State   // 열거형 상수
    { PTROL = 0, TRACE = 1, ATTACK = 2, DIE = 3 , GAMEOVER = 4 , EXPLOSIONDIE = 5 }
    public State state = State.PTROL;   // 시작하자마자 PTROL로

    private Transform playerTr;    // 거리를 재기 위해 선언
    private Transform enemyTr;     // ""
    private Animator animator;
    private float attackDist = 10.0f;         // 5.0f 범위에 들면 공격
    private float traceDist = 20.0f;         // 10.0f 범위에 들면 추적
    public bool isDie = false;              // 사망 여부
    private WaitForSeconds waitTime;        // 기다리는 값 선언
    private EnemyMoveAgent enemyMoveAgent;
    private EnemyFire enemyFire;

    private readonly int hashMove = Animator.StringToHash("IsMove");    // 애니메이터 컨트롤러의 정의된 파라미터의 해시값을 정수로 추출
    private readonly int hashSpeed = Animator.StringToHash("moveSpeed");// 성능 향상을 위함. 미리 문자열 값을 정수 값으로 바꾸어 컴파일러가 읽기 빠르게
    private readonly int hashDie = Animator.StringToHash("DieTrigger");
    private readonly int hashDieIndex = Animator.StringToHash("Die_Index");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDieTrigger");

    void Awake()    //EnemyMoveAgent.cs 보다 빨리 호출되게 하기 위함
    {
        enemyMoveAgent = GetComponent<EnemyMoveAgent>();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) // 유효성 검사. 옵젝이 없으면 안함
            playerTr = player.GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        enemyFire = GetComponent<EnemyFire>();
        animator = GetComponent<Animator>();
        waitTime = new WaitForSeconds(0.3f);    // 0.3초 기다리는 값 할당
    }

    void OnEnable()
    {
        if (!GameManager.instance.isGameOver)
        {
            PlayerDamage.OnPlayerDie += OnPlayerDie;    // 델리게이트 이벤트 연결
            state = State.PTROL;
            animator.SetFloat(hashOffset, Random.Range(0.1f, 1.0f));
            animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 2.0f));
            StartCoroutine(CheckState());
            StartCoroutine(Action());
        }
        else
            return;
    }

    void OnDisable()
    {
        PlayerDamage.OnPlayerDie -= OnPlayerDie;
    }

    void Update()
    {
        animator.SetFloat(hashSpeed, enemyMoveAgent.speed);
    }

    IEnumerator CheckState()    // 현재 거리에 따라 열거형 상수 State 값을 지정. 그 뒤 waitTime 만큼 대기.
    {
        if (GameManager.instance.isGameOver)
            state = State.GAMEOVER;
        while (!isDie && !GameManager.instance.isGameOver)
        {
            if (state == State.DIE || state == State.EXPLOSIONDIE)
                yield break;    // 열거형 상수 State 값이 DIE면 이 함수 종료.
            float dist = (playerTr.position - enemyTr.position).magnitude;
            if (dist <= attackDist)
                state = State.ATTACK;
            else if (dist <= traceDist)
                state = State.TRACE;
            else
                state = State.PTROL;
            yield return waitTime;
        }
    }
    IEnumerator Action()        // waitTime 만큼 대기한 후, switch 실행
    {
        if (GameManager.instance.isGameOver)
            OnPlayerDie();
        while (!isDie && !GameManager.instance.isGameOver)
        {
            yield return waitTime;
            switch (state)
            {
                case State.PTROL:
                    enemyFire.isFire = false;
                    enemyMoveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    if (!enemyFire.isFire)
                        enemyFire.isFire = true;
                    enemyMoveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    break;
                case State.TRACE:
                    enemyFire.isFire = false;
                    enemyMoveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.DIE:
                    enemyFire.isFire = false;
                    enemyMoveAgent.Stop();
                    animator.SetInteger(hashDieIndex, (int)Random.Range(0, 3));
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    gameObject.tag = "Untagged";
                    StartCoroutine(ObjectPoolPush());
                    isDie = true;
                    GameManager.instance.KillScore();
                    break;
                case State.EXPLOSIONDIE:
                    enemyFire.isFire = false;
                    enemyMoveAgent.Stop();
                    animator.SetInteger(hashDieIndex, 0);
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    gameObject.tag = "Untagged";
                    StartCoroutine(ObjectPoolPush());
                    isDie = true;
                    GameManager.instance.KillScore();
                    break;
            }
        }
    }
    IEnumerator ObjectPoolPush()
    {
        yield return new WaitForSeconds(3.0f);
        isDie = false;
        GetComponent<CapsuleCollider>().enabled = true;
        gameObject.tag = "Enemy";
        gameObject.SetActive(false);
    }
    void OnPlayerDie()
    {
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);
    }
}
