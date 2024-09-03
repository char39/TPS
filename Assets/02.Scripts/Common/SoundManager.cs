using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
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
        DontDestroyOnLoad(gameObject);
        soundObjGroup = new GameObject("SoundObjGroup");
        sound_obj = Resources.Load<GameObject>("Prefab/sound_obj");
    }
    public void PlaySound(Vector3 pos, AudioClip clip)
    {
        if (isSoundMute) return;    // 음소거 true일 때 그냥 종료
        var soundobj = Instantiate(sound_obj, soundObjGroup.transform);  // sound_obj 옵젝 동적 생성
        soundobj.transform.position = pos;                 // 소리 위치를 전달받음
        AudioSource audioSource= soundobj.AddComponent<AudioSource>(); // sound_obj에 오디오소스 컴포넌 추가
        audioSource.clip = clip;
        audioSource.minDistance = 20f;  // 3d 음향 효과 최소 거리
        audioSource.maxDistance = 50f;  // 3d 음향 효과 최대 거리
        audioSource.volume = SoundVolume;
        audioSource.Play();
        Destroy(soundobj, clip.length);    // 해당 오디오 클립 길이 만큼 재생한 후 삭제
    }
}
