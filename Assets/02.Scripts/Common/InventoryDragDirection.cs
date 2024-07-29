using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDirction : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    private Vector2 startPosition;
    public Transform itemListTr;
    public Transform itemTr;
    public bool isDown = false;

    void Start()
    {
        itemListTr = GameObject.Find("ItemList").transform;
        itemTr = GetComponent<Transform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = Input.mousePosition;
        // startPosition = Input.touches[0].position;   // 모바일 터치
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 endPosition = Input.mousePosition;
        Vector2 direction = endPosition - startPosition;

        // 아래쪽으로 드래그했는지 판별
        if (direction.y < 0)
        {
            Debug.Log("아래쪽으로 드래그했습니다.");
            //Drag.draggingItem.GetComponent<ItemInfo>().itemData = null;
            isDown = true;
        }
        else
        {
            Debug.Log("다른 방향으로 드래그했습니다.");
            isDown = false;
        }
    }
}
