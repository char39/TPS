using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SoundManager : MonoBehaviourPun
{
    public static SoundManager Instance;
    public float SoundVolume = 1.0f;    // 사운드 볼륨
    public bool isSoundMute = false;    // 음소거 여부
    GameObject soundObjGroup;
    GameObject sound_obj;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        soundObjGroup = new GameObject("SoundObjGroup");
    }
    [PunRPC]
    public void PlaySound(Vector3 pos, AudioClip clip)
    {
        if (!photonView.IsMine) return;
        if (isSoundMute) return;    // 음소거 true일 때 그냥 종료
        var soundobj = PhotonNetwork.Instantiate("sound_obj", soundObjGroup.transform.position, soundObjGroup.transform.rotation);  // sound_obj 옵젝 동적 생성
        soundobj.transform.position = pos;                 // 소리 위치를 전달받음
        AudioSource audioSource= soundobj.AddComponent<AudioSource>(); // sound_obj에 오디오소스 컴포넌 추가
        audioSource.clip = clip;
        audioSource.minDistance = 20f;  // 3d 음향 효과 최소 거리
        audioSource.maxDistance = 50f;  // 3d 음향 효과 최대 거리
        audioSource.volume = SoundVolume;
        audioSource.Play();
        StartCoroutine(DestroySoundObj(soundobj, clip.length));
    }
    public IEnumerator DestroySoundObj(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(obj);
    }

}
