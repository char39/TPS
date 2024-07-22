using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class EnemySwatAI : MonoBehaviour
{
    public enum State   // 열거형 상수
    { PTROL = 0, TRACE = 1, ATTACK = 2, DIE = 3, GAMEOVER = 4 }
    public State state = State.PTROL;

    private Transform playerTr;
    private Transform enemySwatTr;
    private Animator animator;
    private float attackDist = 10.0f;
    private float traceDist = 20.0f;
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
        waitTime = new WaitForSeconds(0.3f);
    }

    void OnEnable()
    {
        if (!GameManager.instance.isGameOver)
        {
            state = State.PTROL;
            animator.SetFloat(hashOffset, Random.Range(0.1f, 1.0f));
            animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 2.0f));
            StartCoroutine(CheckState());
            StartCoroutine(Action());
        }
    }

    void Update()
    {
        animator.SetFloat(hashSpeed, enemySwatMoveAgent.speed);
    }

    IEnumerator CheckState()
    {
        if (GameManager.instance.isGameOver)
            state = State.GAMEOVER;
        while (!isDie && !GameManager.instance.isGameOver)
        {
            if (state == State.DIE)
                yield break;
            float dist = (playerTr.position - enemySwatTr.position).magnitude;
            if (dist <= attackDist)
                state = State.ATTACK;
            else if (dist <= traceDist)
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
