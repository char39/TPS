using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager_script : MonoBehaviour
{
    public static ObjectPoolingManager_script poolingManager;
    [SerializeField]private GameObject bulletPrefab;
    [SerializeField] private GameObject e_bulletPrefab;
    public int maxPool = 10;    //오브젝트 풀에 생성 할 개수
    public int maxPool_e = 10;
    public List<GameObject> bulletPoolList;
    public List<GameObject> e_bulletPoolList;

    void Awake()
    {
        if (poolingManager == null)
            poolingManager = this;
        else if (poolingManager != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        bulletPrefab = Resources.Load<GameObject>("Prefab/Bullet");
        e_bulletPrefab = Resources.Load<GameObject>("Prefab/E_Bullet");
        CreateBulletPool(); //오브젝트 풀링 생성 함수
        CreateE_BulletPool();
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

    void CreateE_BulletPool()
    {
        GameObject enemyBulletGroup = new GameObject("EnemyBulletGroup");
        for (int i = 0; i < 10; i++)
        {
            var bullets = Instantiate(e_bulletPrefab, enemyBulletGroup.transform);
            bullets.name = $"e_{(i + 1).ToString()} 발";
            bullets.SetActive(false);
            e_bulletPoolList.Add(bullets);
        }
    }
    public GameObject GetE_BulletPool()
    {
        for (int i = 0; i < e_bulletPoolList.Count; i++)
        {
            if (e_bulletPoolList[i].activeSelf == false)  // 비활성 되어있다면 activeSelf는 활성화, 비활성화 여부를 알려줌
            {
                return e_bulletPoolList[i];   // 비활성화 되어있는 얘만 리턴
            }
        }
        return null;    // 아니면 null
    }
}
