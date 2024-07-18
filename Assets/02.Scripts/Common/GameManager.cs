using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Camera mainCamera;
    private Transform mainCam_tr;
    private Transform mainCam_tr_startPos;

    void Start()
    {
        instance = this;
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        mainCam_tr = mainCamera.transform;
    }

    public IEnumerator CameraShake()
    {
        for (int i = 30; i > 0; i--)
        {
            float ran = Random.Range(-0.1f, 0.1f) * i * 0.1f;
            Vector3 changePos = new Vector3(ran, ran, ran);
            mainCamera.transform.position += changePos;

            float angle = Random.Range(-2f, 2f) * i * 0.1f;
            Quaternion changeRot = Quaternion.Euler(angle, angle, angle);
            mainCamera.transform.rotation *= changeRot;

            yield return new WaitForSeconds(0.002f);
        }
        mainCamera.transform.position = mainCam_tr.position;
        mainCamera.transform.rotation = mainCam_tr.rotation;
    }
}
