using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmo : MonoBehaviour
{
    public enum Type { NORMAL, WAYPOINT }
    private const string wayPointFile = "Enemy";
    public Type type = Type.NORMAL;             // type을 NORMAL로 지정
    public Color color;
    public float radius;

    void Start()
    {

    }

    private void OnDrawGizmos()
    {
        if (type == Type.NORMAL)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
        }
        else
        {
            Gizmos.color = color;
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.0f, wayPointFile, true);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
