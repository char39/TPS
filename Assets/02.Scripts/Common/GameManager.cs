using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Camera mainCamera;
    private Transform mainCam_tr;
    public bool isGameOver = false;
    CanvasGroup inventoryOpen;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        mainCam_tr = mainCamera.transform;
        inventoryOpen = GameObject.Find("Inventory").GetComponent<CanvasGroup>();
        InventoryOnOff(false);
    }

    public IEnumerator CameraShake()
    {
        mainCam_tr = mainCamera.transform;
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

    public bool isPaused = false;
    public bool isOpened = false;

    public void OnPauseClick()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0.0f : 1.0f;
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var scripts = playerObj.GetComponents<MonoBehaviour>();         // Player에게 있는 모든 스크립트들을 Get.
        foreach (var script in scripts)
            script.enabled = !isPaused;
        var canvasGroup = GameObject.Find("Canvas_UI").transform.GetChild(3).GetComponent<CanvasGroup>();
        var canvasGroupBlood = GameObject.Find("Canvas_UI").transform.GetChild(0).GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
        canvasGroupBlood.blocksRaycasts = !isPaused;
    }
    
    public void InventoryButtonClick()
    {
        isOpened = !isOpened;
        InventoryOnOff(isOpened);
    }
    public void InventoryOnOff(bool isOpen)
    {
        inventoryOpen.alpha = isOpen ? 1.0f : 0.0f;
        inventoryOpen.interactable = isOpen ? true : false;
        inventoryOpen.blocksRaycasts = isOpen ? true : false;
    }
}
