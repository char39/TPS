using System.Collections;
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
        if (photonView.IsMine)
        {
            speed = 1000f;
            damage = 25f;
            rb.AddForce(transform.forward * speed);
            photonView.RPC("BulletDestory_M", RpcTarget.All);
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, curRot, Time.deltaTime * 10.0f);
        }
    }

    [PunRPC]
    void BulletDestory_M()
    {
        StartCoroutine(BulletDestroy());
    }
    IEnumerator BulletDestroy()
    {
        yield return new WaitForSeconds(3.0f);
        trail.Clear();
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }





/* 
    private void OnEnable() // 옵젝 켜질 때마다 실행
    {
        if (GameManager.instance != null && GameManager.instance.gameData != null)
            damage = GameManager.instance.gameData.damage;
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
    } */

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
