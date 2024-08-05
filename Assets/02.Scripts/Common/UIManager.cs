using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public void OnClickPlayButton()
    {
        SceneManager.LoadScene("SceneLoader");
    }
    public void OnClickQuitButton()
    {
        #if UNITY_EDITOR    // 유니티 에디터에서 실행 중인지 확인
        EditorApplication.isPlaying = false; // 에디터 상에서 플레이 모드 종료
        #else               // 유니티 에디터가 아닌 경우
        Application.Quit(); // 어플리케이션 종료
        #endif
    }
}


//SceneManager.LoadScene("Level_1");
//SceneManager.LoadScene("BattleField_Scene", LoadSceneMode.Additive); // LoadSceneMode.Additive : 씬을 추가로 로드
//SceneManager.LoadScene("BattleField_Scene", LoadSceneMode.Single); // LoadSceneMode.Single : 기존에 로드된 모든 씬들을 삭제 후 씬을 로드