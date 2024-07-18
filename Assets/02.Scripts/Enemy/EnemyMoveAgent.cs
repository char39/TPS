using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyMoveAgent : MonoBehaviour
{
    // 패트롤 지점을 담기 위한 List Generic(일반형) 변수
    public List<Transform> wayPointList;
    public int nextIndex = 0;       // 다음 순찰 지점의 배열 Index 값
    [SerializeField] private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        var group = GameObject.Find("WayPointGroup");
        if (group != null)  // 유효성 검사. 존재하는지 안하는지
        {
            group.GetComponentsInChildren<Transform>(wayPointList); // WayPointGroup 옵젝부터 그 자식옵젝 전부 get
            wayPointList.RemoveAt(0);   // 자식들만 쓸 것이기 때문에 본인 옵젝은 리스트에서 제거함 (인덱스와 함께 같이 제거)
        }
        MovewayPoint();
    }

    void Update()
    {
        if (agent.remainingDistance <= 0.5f)    // 다음 도착지점까지 거리가 0.5 이하일 경우
        {
            nextIndex = ++nextIndex % wayPointList.Count;
            MovewayPoint();
        }
    }

    void MovewayPoint()
    {
        if (agent.isPathStale) return;  // 최단 경로 계산이 끝나지 않거나 길을 잃어버린 경우
        agent.destination = wayPointList[nextIndex].position;   // 경로 추적 지점 = 리스트에 담았던 transform
        agent.isStopped = false;
    }
}
