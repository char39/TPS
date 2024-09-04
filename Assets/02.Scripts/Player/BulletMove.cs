using Photon.Pun;
using UnityEngine;

public class BulletMove : MonoBehaviourPun, IPunObservable
{
    private float speed;
    public float damage;
    private Rigidbody rb;
    private TrailRenderer trail;

    private Vector3 curPos;
    private Quaternion curRot;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            speed = 1000f;
            damage = 25f;
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, curRot, Time.deltaTime * 10.0f);
        }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
