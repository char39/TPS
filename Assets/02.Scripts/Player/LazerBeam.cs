using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LazerBeam : MonoBehaviour
{
    public static LazerBeam instance;
    private Transform tr;
    private LineRenderer line;

    void Start()
    {
        instance = this;
        tr = transform;
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.enabled = false;
    }

    public void PlayerLazerBeam()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward);  // 광선을 미리 생성
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);

        line.SetPosition(0, tr.InverseTransformPoint(ray.origin));      // 라인 렌더러 첫번째 점의 위치를 설정. 월드 좌표 방향을 로컬 좌표 방향으로 변경.
        if (Physics.Raycast(ray, out hit, 100f))                        // 어떤 물체에 광선이 맞았을 때 위치를 Line Renderer의 끝점으로 설정
            line.SetPosition(1, tr.InverseTransformPoint(hit.point));
        else                                                            // 안맞았을 때 끝점을 100으로 설정.
            line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100f)));
        StartCoroutine(ShowLazerBeam());

    }
    IEnumerator ShowLazerBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(0.05f);
        line.enabled = false;
    }
}
