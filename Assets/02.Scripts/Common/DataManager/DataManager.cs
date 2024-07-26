using System.IO;                                        // 파일 입출력을 위한 Namespace
using System.Runtime.Serialization.Formatters.Binary;   // 바이너리 형식으로 사용하기 위한 Namespace
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataInfo;         // GameData.cs의 네임스페이스

public class DataManager : MonoBehaviour
{
    [SerializeField] string dataPath;   // 저장 경로
    public void Initialized()           // 저장 경로를 초기화하는 메서드
    {
        dataPath = Application.persistentDataPath + "/gameData.dat";     // 파일 저장 경로와 파일명 지정. persistentDataPath : 공용폴더에 저장
    }

    public void Save(GameData gameData)
    {
        BinaryFormatter binaryFormatter = new();            // 코드 2번 줄의 네임스페이스를 사용
        FileStream fileStream = File.Create(dataPath);      // 코드 1번 줄의 네임스페이스를 사용

        GameData data = new();                              // 파일에 저장할 클래스에 데이터 할당
        data.killCounts = gameData.killCounts;
        data.hp = gameData.hp;
        data.speed = gameData.speed;
        data.damage = gameData.damage;
        data.equipItem = gameData.equipItem;

        binaryFormatter.Serialize(fileStream, data);        // 파일에 데이터 저장
        fileStream.Close();                                 // 파일 닫기. 닫지 않는다면 메모리를 많이 사용하기에 닫아야 한다.
    }

    public GameData Load()
    {
        if (File.Exists(dataPath))          // 유효성 검사
        {
            BinaryFormatter binaryFormatter = new();    
            FileStream fileStream = File.Open(dataPath, FileMode.Open);         // 파일 열기
            GameData data = (GameData)binaryFormatter.Deserialize(fileStream);  // 파일에서 데이터 불러오기
            fileStream.Close();
            return data;
        }
        else                                // 파일이 없다면 새로운 데이터를 생성하여 반환
        {
            GameData data = new();
            return data;
        }
    }
    
}
