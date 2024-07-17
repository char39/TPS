using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager_script : MonoBehaviour
{
    public static ObjectPoolingManager_script poolingManager;
    [SerializeField]private GameObject bulletPrefab;
    public int maxPool = 10;    //오브젝트 풀에 생성 할 개수
    public List<GameObject> bulletPoolList;

    void Awake()
    {
        if (poolingManager == null)
            poolingManager = this;
        else if (poolingManager != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        bulletPrefab = Resources.Load<GameObject>("Prefab/Bullet");
        CreateBulletPool(); //오브젝트 풀링 생성 함수
    }

    void CreateBulletPool()
    {
        GameObject playerBulletGroup = new GameObject("PlayerBulletGroup");
        for (int i = 0; i < 10; i++)
        {
            var bullets = Instantiate(bulletPrefab, playerBulletGroup.transform);
            bullets.name = $"{(i + 1).ToString()} 발";
            bullets.SetActive(false);
            bulletPoolList.Add(bullets);
        }
    }
    public GameObject GetBulletPool()
    {
        for (int i = 0; i < bulletPoolList.Count; i++)
        {
            if (bulletPoolList[i].activeSelf == false)  // 비활성 되어있다면 activeSelf는 활성화, 비활성화 여부를 알려줌
            {
                return bulletPoolList[i];   // 비활성화 되어있는 얘만 리턴
            }
        }
        return null;    // 아니면 null
    }

}
