using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    // 안씀
    public float viewRange = 14.0f;     // 적 캐릭터 추적 가능 거리
    [Range(0, 360)] public float viewAngle = 120f;      // 적 캐릭터 추적 시야각
    [SerializeField] private Transform enemyTr;
    [SerializeField] private Transform playerTr;
    [SerializeField] private int layerMask;
    [SerializeField] private int playerLayer;
    [SerializeField] private int boxesLayer;
    [SerializeField] private int barrelLayer;
    [SerializeField] private int wallLayer;
    

    void Start()
    {
        enemyTr = transform;
        playerTr = GameObject.FindWithTag("Player").transform;

        playerLayer = LayerMask.NameToLayer("Player");
        boxesLayer = LayerMask.NameToLayer("Boxes");
        barrelLayer = LayerMask.NameToLayer("Barrel");
        wallLayer = LayerMask.NameToLayer("Wall");
        layerMask = 1 << playerLayer | 1 << boxesLayer | 1 << barrelLayer | 1 << wallLayer;
    }

    public Vector3 CirclePoint(float angle)                 // 원주 위의 한 점을 계산하는 함수
    {
        angle += transform.eulerAngles.y;                   // 적 캐릭터의 현재 y축 회전값을 더함
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad)); // 삼각함수를 이용하여 원의 좌표를 구함
    }
    public bool isTracePlayer()
    {
        bool isTrace = false;
        Collider[] cols = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);     // 주변에 있는 플레이어를 찾는다.
        if (cols.Length == 1)
        {
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;         // 적 캐릭터에서 플레이어를 향하는 벡터
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)               // 적 캐릭터의 시야각 내에 플레이어가 있는지 확인
                isTrace = true;
        }
        return isTrace;
    }
    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;
        Vector3 dir = (playerTr.position - enemyTr.position).normalized;
        if (Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
            isView = hit.collider.CompareTag("Player");
        return isView;
    }
    
}
