using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public CanvasGroup fadeCG;
    [Range(0.5f, 2.0f)] public float fadeDuration = 1.0f;
    public Dictionary<string, LoadSceneMode> loadScenes = new();

    void InitialSceneInfo()     // 씬 정보 이니셜
    {
        loadScenes.Add("Level_1", LoadSceneMode.Additive);
        loadScenes.Add("BattleField_Scene", LoadSceneMode.Additive);
    }

    IEnumerator Start()
    {
        fadeCG = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<CanvasGroup>();
        fadeCG.alpha = 1.0f;
        InitialSceneInfo();
        foreach (var loadScene in loadScenes)
        {
            yield return StartCoroutine(LoadScene(loadScene.Key, loadScene.Value));
        }
        StartCoroutine(Fade(0.0f));
    }

    IEnumerator LoadScene(string sceneName, LoadSceneMode mode)         // 씬 로드 코루틴
    {
        yield return SceneManager.LoadSceneAsync(sceneName, mode);      // 씬 로드. 비동기 방식 (로드되는 중에도 게임이 계속 실행하게 함)
        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);   // 로드된 씬을 가져옴
        SceneManager.SetActiveScene(loadedScene);                                   // 로드된 씬을 활성화
    }

    IEnumerator Fade(float finalAlpha)
    {
        //                                                                          // "Level_1"에는 Lighting Data가 있음
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));        // "Level_1" 씬을 활성화
        fadeCG.blocksRaycasts = true;                                               // 마우스 클릭을 막음
        float fadeSpeed = Mathf.Abs(fadeCG.alpha - finalAlpha) / fadeDuration;      // fadeSpeed = (현재 알파값 - 목표 알파값) / fadeDuration
        while (!Mathf.Approximately(fadeCG.alpha, finalAlpha))                          // 현재 알파값이 목표 알파값과 같지 않다면 (아직 불투명하다면)
        {
            fadeCG.alpha = Mathf.MoveTowards(fadeCG.alpha, finalAlpha, fadeSpeed * Time.deltaTime);     // 현재 알파값을 목표 알파값으로 이동
            yield return null;                                                                          // 다음 프레임까지 대기
        }
        fadeCG.blocksRaycasts = false;                                              // 마우스 클릭을 허용
        SceneManager.UnloadSceneAsync("SceneLoader");                               // "SceneLoader" 씬을 언로드 (Async : 비동기, 독립적으로)
    }

    void Update()
    {
        
    }
}
