using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCamTr;
    private Transform tr;
    void Start()
    {
        mainCamTr = Camera.main.transform;
        tr = transform;   
    }

    void Update()
    {
        tr.LookAt(mainCamTr);
        Quaternion currentRot = tr.rotation;
        tr.rotation = currentRot * Quaternion.Euler(0, 180, 0);
    }
}
