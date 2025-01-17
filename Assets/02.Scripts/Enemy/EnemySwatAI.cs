using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class EnemySwatAI : MonoBehaviour
{
    public enum State   // 열거형 상수
    { PTROL = 0, TRACE = 1, ATTACK = 2, DIE = 3, GAMEOVER = 4, EXPLOSIONDIE = 5 }
    public State state = State.PTROL;

    private Transform playerTr;
    private Transform enemySwatTr;
    private Animator animator;

    private EnemyFOV enemyFOV;

    private float attackDist = 7.0f;
    private float traceDist = 14.0f;
    public bool isDie = false;
    private WaitForSeconds waitTime;
    private EnemySwatMoveAgent enemySwatMoveAgent;
    private EnemySwatFire enemySwatFire;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("moveSpeed");
    private readonly int hashDie = Animator.StringToHash("DieTrigger");
    private readonly int hashDieIndex = Animator.StringToHash("Die_Index");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDieTrigger");

    void Awake()
    {
        enemySwatMoveAgent = GetComponent<EnemySwatMoveAgent>();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTr = player.GetComponent<Transform>();
        enemySwatTr = GetComponent<Transform>();
        enemySwatFire = GetComponent<EnemySwatFire>();
        animator = GetComponent<Animator>();
        enemyFOV = GetComponent<EnemyFOV>();
        waitTime = new WaitForSeconds(0.3f);
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
        animator.SetFloat(hashSpeed, enemySwatMoveAgent.speed);
    }

    IEnumerator CheckState()
    {
        yield return new WaitForSeconds(1.0f);
        if (GameManager.instance.isGameOver)
            state = State.GAMEOVER;
        while (!isDie && !GameManager.instance.isGameOver)
        {
            if (state == State.DIE || state == State.EXPLOSIONDIE)
                yield break;
            float dist = (playerTr.position - enemySwatTr.position).magnitude;
            if (dist <= attackDist)
            {
                if (enemyFOV.isViewPlayer())
                    state = State.ATTACK;
                //else
                //    state = State.TRACE;
            }
            else if (enemyFOV.isTracePlayer())
                state = State.TRACE;
            else
                state = State.PTROL;
            yield return waitTime;
        }
    }
    IEnumerator Action()
    {
        if (GameManager.instance.isGameOver)
            OnPlayerDie();
        while (!isDie && !GameManager.instance.isGameOver)
        {
            yield return waitTime;
            switch (state)
            {
                case State.PTROL:
                    enemySwatFire.isFire = false;
                    enemySwatMoveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    if (!enemySwatFire.isFire)
                        enemySwatFire.isFire = true;
                    enemySwatMoveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    break;
                case State.TRACE:
                    enemySwatFire.isFire = false;
                    enemySwatMoveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.DIE:
                    enemySwatFire.isFire = false;
                    enemySwatMoveAgent.Stop();
                    animator.SetInteger(hashDieIndex, (int)Random.Range(0, 3));
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    gameObject.tag = "Untagged";
                    StartCoroutine(ObjectPoolPush());
                    isDie = true;
                    GameManager.instance.KillScore();
                    break;
                case State.EXPLOSIONDIE:
                    enemySwatFire.isFire = false;
                    enemySwatMoveAgent.Stop();
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
        gameObject.tag = "EnemySwat";
        gameObject.SetActive(false);
    }
    void OnPlayerDie()
    {
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);
    }

}
