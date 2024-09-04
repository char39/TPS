using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyMoveAgent : MonoBehaviourPun, IPunObservable
{
    
    // 패트롤 지점을 담기 위한 List Generic(일반형) 변수
    public List<Transform> wayPointList;
    public int nextIndex = 0;       // 다음 순찰 지점의 배열 Index 값
    [SerializeField] private NavMeshAgent agent;
    private Rigidbody rb;
    private EnemyAI enemyAI;

    private Vector3 currentPos;
    private Quaternion currentRot;

    private readonly float patrollSpeed = 2.5f;
    private readonly float traceSpeed = 4.0f;
    private float damping = 1.0f;   // 회전할 때의 속도를 조절하는 계수
    private bool _patrolling;
    private Transform enemyTr;
    public bool patrolling      // 프로퍼티
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrollSpeed;
                damping = 1.0f;
                MovewayPoint();
            }
        }
    }
    private Vector3 _traceTarget;
    public Vector3 traceTarget  // 프로퍼티
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }

    public float speed          // 프로퍼티
    {
        get { return agent.velocity.magnitude; }
        
    }


    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.autoBraking = false;
        agent.updateRotation = false;   // agent를 이용하여 회전하는 기능을 비활성. 회전이 부드럽지 않기 때문.
        var group = GameObject.Find("WayPointGroup");
        if (group != null)  // 유효성 검사. 존재하는지 안하는지
        {
            group.GetComponentsInChildren<Transform>(wayPointList); // WayPointGroup 옵젝부터 그 자식옵젝 전부 get
            wayPointList.RemoveAt(0);   // 자식들만 쓸 것이기 때문에 본인 옵젝은 리스트에서 제거함 (인덱스와 함께 같이 제거)
        }
        nextIndex = Random.Range(0, wayPointList.Count);
        MovewayPoint();

        currentPos = enemyTr.position;
        currentRot = enemyTr.rotation;
    }

    void Update()
    {
        if (GameManager.instance.isGameOver)
            Stop();
        if (PhotonNetwork.IsConnected)
        {
            if (!agent.isStopped && !enemyAI.isDie)   // agent가 움직이고 있다면
            {
                Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);    // NavMeshAgent가 가야되는 방향 벡터를 Quaternion 타입의 각도로 변환.
                enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);   // 보간 함수를 이용하여 부드럽게 회전시킴.
            }
            if (!_patrolling) return;
            FindWayPoint();
            if (!photonView.IsMine && currentPos != null && currentRot != null)
            {
                enemyTr.position = Vector3.Lerp(enemyTr.position, currentPos, Time.deltaTime * 10.0f);
                enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, currentRot, Time.deltaTime * 10.0f);
            }
        }
    }

    private void FindWayPoint()             // 수색 경로를 탐색
    {
        if (agent.remainingDistance <= 0.5f)    // 다음 도착지점까지 거리가 0.5 이하일 경우
        {
            //nextIndex = ++nextIndex % wayPointList.Count;
            nextIndex = Random.Range(0, wayPointList.Count);
            MovewayPoint();
        }
    }
    private void MovewayPoint()             // 수색 경로를 추적
    {
        if (agent.isPathStale)
            return;  // 최단 경로 계산이 끝나지 않거나 길을 잃어버린 경우
        agent.destination = wayPointList[nextIndex].position;   // 경로 추적 지점 = 리스트에 담았던 transform
        agent.isStopped = false;
        rb.isKinematic = false;
    }
    private void TraceTarget(Vector3 pos)   // 타겟을 추적
    {
        if (agent.isPathStale) return;
            agent.destination = pos;
        agent.isStopped = false;
        rb.isKinematic = false;
    }
    public void Stop()  // 추적 중지
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.destination = Vector3.zero;
        rb.isKinematic = true;
        _patrolling = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(enemyTr.position);
            stream.SendNext(enemyTr.rotation);
        }
        else
        {
            currentPos = (Vector3)stream.ReceiveNext();
            currentRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
