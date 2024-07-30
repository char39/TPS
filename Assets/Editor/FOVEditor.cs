using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()       // Scene 뷰에 그려지는 함수
    {
        EnemyFOV fov = (EnemyFOV)target;    // EnemyFOV.cs를 참조한다.
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);  // 시야각의 시작점
        Handles.color = Color.white;                            // 색상 설정
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRange);    // 원의 외곽선 그리기. (원의 중심, 원의 방향, 원의 반지름)
        Handles.color = new Color(1, 1, 1, 0.2f);               // 색상 설정
        Handles.DrawSolidArc(fov.transform.position, Vector3.up, fromAnglePos, fov.viewAngle, fov.viewRange);   // 부채꼴 모양의 시야각 그리기. (원의 중심, 원의 방향, 부채꼴의 시작점, 부채꼴의 각도, 부채꼴의 반지름)
        Handles.Label(fov.transform.position + (fov.transform.forward * 2.0f), fov.viewAngle.ToString());       // 시야각의 각도를 Scene 뷰에 표시
        
    }





}
