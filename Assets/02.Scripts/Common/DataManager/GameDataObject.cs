using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataInfo;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "Create GameData", order = 1)]

public class GameDataObject : ScriptableObject     // ScriptableObject를 상속받아야 에디터에서 생성 가능
{
    public int killCounts = 0;
    public float hp = 120f;
    public float damage = 25f;
    public float speed = 6.0f;
    public List<Item> equipItem = new();
}
