using UnityEngine;

public class BulletMove : MonoBehaviour
{
    private float speed;
    public float damage;
    private Rigidbody rb;
    private TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        speed = 1000f;
        damage = 25f;
        rb = GetComponent<Rigidbody>();
        //gameObject.SetActive(false);
    }
    void BulletDisable()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable() // 옵젝 켜질 때마다 실행
    {
        if (GameManager.instance != null && GameManager.instance.gameData != null)
            damage = GameManager.instance.gameData.damage;
        Invoke(nameof(BulletDisable), 3.0f);
        rb.AddForce(transform.forward * speed);
        GameManager.OnItemChange += UpdateSetup;    // 이벤트 등록
    }
    void UpdateSetup()
    {
        damage = GameManager.instance.gameData.damage;
    }
    private void OnDisable()
    {
        trail.Clear();
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        rb.Sleep();
    }
}
