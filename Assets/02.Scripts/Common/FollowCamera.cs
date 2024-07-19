using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Transform target;       // 따라다닐 대상
    private float height;           // 카메라의 높이
    private float distance;         // 타겟과의 거리
    private float moveDamping;      // 카메라의 이동 움직임을 부드럽게 적용하는 값
    private float rotDamping;       // 카메라의 회전 움직임을 부드럽게 적용하는 값
    private Transform camTr;        // 카메라 본인의 transform
    private float targetOffset;     // 타겟에서의 카메라 높이값

    void Start()
    {
        StartComponent();
        StartVars();
    }
    private void StartComponent()       // 컴포넌트 초기값 할당
    {
        camTr = GetComponent<Transform>();
        target = GameObject.FindWithTag("Player").transform;
    }
    private void StartVars()            // 변수 초기값 할당
    {
        height = 5.0f;
        distance = 7.0f;
        moveDamping = 10.0f;
        rotDamping = 15.0f;
        targetOffset = 2.0f;
    }



    void LateUpdate()   // LateUpdate() : 모든 Update 메서드가 호출된 후에 실행되는 메서드.
    {
        CameraFollow();
    }

    private void CameraFollow()         // 카메라가 타겟을 대상으로 부드럽게 움직이는 메서드
    {
        /* 이렇게 할 수도 있다.
        var camPos = target.position - (target.forward * distance) + (target.up * height);
        */
        var camPos = target.position - (Vector3.forward * distance) + (Vector3.up * height);    // 타겟 포지션에서 distance만큼 뒤에 위치 + height 높이 만큼 위에 위치
        camTr.position = Vector3.Slerp(camTr.position, camPos, Time.deltaTime * moveDamping);               // 곡면 보간. 본인 위치에서, camPos까지, moveDamping * 시간만큼 부드럽게 움직임
        camTr.rotation = Quaternion.Slerp(camTr.rotation, target.rotation, Time.deltaTime * rotDamping);    // 본인 회전값에서 타겟의 회전
        camTr.LookAt(target.position + (target.up * targetOffset));                             // 타겟 위치에서 targetOffset만큼 위로 올림.
    }
    /*
    private void OnDrawGizmos()         // Scene에서 Debug상의 여러 색상이나 선 등을 그려주는 메서드. 콜백 함수
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target.position + (target.up * targetOffset), 0.1f);          // (그리는 위치, 반지름)
        Gizmos.DrawLine(target.position + (target.up * targetOffset), camTr.position);  // (여기부터, 여기까지) 선을 그림
    }
    */
}
