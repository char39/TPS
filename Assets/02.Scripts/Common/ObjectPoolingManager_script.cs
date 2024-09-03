using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ObjectPoolingManager_script : MonoBehaviourPunCallbacks
{
    public static ObjectPoolingManager_script poolingManager;
    private GameObject bulletPrefab;
    private GameObject e_bulletPrefab;
    private GameObject enemyPrefab;
    private GameObject enemySwatPrefab;
    private int maxPool = 1;    //오브젝트 풀에 생성 할 개수
    private int maxPool_e = 25;
    private int maxPool_Enemy = 10;
    private int maxPool_EnemySwat = 10;
    public List<GameObject> bulletPoolList;
    public List<GameObject> e_bulletPoolList;
    public List<GameObject> enemyPoolList;
    public List<GameObject> enemySwatPoolList;
    public List<Transform> spawnPointList;

    void Awake()
    {
        if (poolingManager == null)
            poolingManager = this;
        else if (poolingManager != this)
            Destroy(gameObject);

        bulletPrefab = Resources.Load<GameObject>("Prefab/Bullet");
        e_bulletPrefab = Resources.Load<GameObject>("Prefab/E_Bullet");
        enemyPrefab = Resources.Load<GameObject>("Prefab/Enemy");
        enemySwatPrefab = Resources.Load<GameObject>("Prefab/EnemySwat");
    }
    
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        CreateE_BulletPool();
        CreateEnemyPool();
        FindSpawnPoint();
    }

    void FindSpawnPoint()
    {
        var spawnpoints = GameObject.Find("SpawnPoints");
        if (spawnpoints != null)
            spawnpoints.GetComponentsInChildren(spawnPointList);
        spawnPointList.RemoveAt(0); // 부모 옵젝만 리스트에서 지우기
        if (spawnPointList.Count > 0)
        {
            StartCoroutine(CreateEnemy());
            //StartCoroutine(CreateEnemySwat());
        }
    }
    IEnumerator CreateEnemy()
    {
        while (!GameManager.instance.isGameOver)
        {
            yield return new WaitForSeconds(3.0f);          // 3.0f 초 대기
            if (GameManager.instance.isGameOver)            // 게임오버라면
                yield break;                                    // while문 탈출
            foreach (GameObject enemy in enemyPoolList)     
            {
                if (!enemy.activeSelf)
                {
                    int index = Random.Range(0, spawnPointList.Count);
                    enemy.transform.position = spawnPointList[index].position;
                    enemy.transform.rotation = spawnPointList[index].rotation;
                    enemy.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
    IEnumerator CreateEnemySwat()
    {
        while (!GameManager.instance.isGameOver)
        {
            yield return new WaitForSeconds(3.0f);          // 3.0f 초 대기
            if (GameManager.instance.isGameOver)            // 게임오버라면
                yield break;                                    // while문 탈출
            foreach (GameObject enemy in enemySwatPoolList)     
            {
                if (!enemy.activeSelf)
                {
                    int index = Random.Range(0, spawnPointList.Count);
                    enemy.transform.position = spawnPointList[index].position;
                    enemy.transform.rotation = spawnPointList[index].rotation;
                    enemy.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    void CreateBulletPool()
    {
        GameObject playerBulletGroup = new GameObject("PlayerBulletGroup");
        for (int i = 0; i < maxPool; i++)
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
        for (int i = 0; i < maxPool_e; i++)
        {
            GameObject bullets = PhotonNetwork.InstantiateRoomObject("E_Bullet", Vector3.zero, Quaternion.identity);
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

    void CreateEnemyPool()
    {
        for (int i = 0; i < maxPool_Enemy; i++)
        {
            GameObject enemys = PhotonNetwork.InstantiateRoomObject("Enemy", Vector3.zero, Quaternion.identity);
            enemys.name = $"enemy {(i + 1).ToString()}";
            enemys.SetActive(false);
            enemyPoolList.Add(enemys);
        }
    }
    public GameObject GetEnemyPool()
    {
        for (int i = 0; i < enemyPoolList.Count; i++)
        {
            if (enemyPoolList[i].activeSelf == false)  // 비활성 되어있다면 activeSelf는 활성화, 비활성화 여부를 알려줌
            {
                return enemyPoolList[i];   // 비활성화 되어있는 얘만 리턴
            }
        }
        return null;    // 아니면 null
    }

    [PunRPC]
    public void SyncEnemyState(int index, bool state)
    {
        if (index >= 0 && index < enemyPoolList.Count)
        {
            enemyPoolList[index].SetActive(state);
        }
    }

    void CreateEnemySwatPool()
    {
        GameObject enemySwatGroup = new GameObject("EnemySwatGroup");
        for (int i = 0; i < maxPool_EnemySwat; i++)
        {
            var enemys = Instantiate(enemySwatPrefab, enemySwatGroup.transform);
            enemys.name = $"enemy {(i + 1).ToString()}";
            enemys.SetActive(false);
            enemySwatPoolList.Add(enemys);
        }
    }
    public GameObject GetEnemySwatPool()
    {
        for (int i = 0; i < enemySwatPoolList.Count; i++)
        {
            if (enemySwatPoolList[i].activeSelf == false)
            {
                return enemySwatPoolList[i];
            }
        }
        return null;
    }

}
