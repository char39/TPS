using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonInitialSetting : MonoBehaviourPunCallbacks
{
    public GameObject LobbyObj;             // Lobby UI
    public GameObject roomItem;             // Room Item UI
    public GameObject scrollContent;        // Room Item을 표시할 Content UI
    public string gameVersion = "1.0";      // 게임 버전
    public InputField userID;               // 사용자 ID 입력 UI
    public InputField roomName;             // Room 입력 UI
    public Button joinRandomRoom;           // Join Random Room 버튼 UI
    public Button joinRoom;                 // Join Room 버튼 UI

    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            joinRandomRoom.interactable = false;                // Join Random Room 버튼 비활성화
            joinRoom.interactable = false;                      // Join Room 버튼 비활성화
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connect to Master Server...");
            roomName.text = "Room_" + Random.Range(0, 999).ToString("000");
        }
    }

    public override void OnConnectedToMaster()          // Master Server에 연결되었을 때 호출되는 콜백함수
    {
        Debug.Log("Online : 서버에 연결 되었습니다.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()                // Lobby에 입장했을 때 호출되는 콜백함수
    {
        Debug.Log("Online : 로비에 입장했습니다.");
        userID.text = GetUserID();
        joinRandomRoom.interactable = true;                 // Join Random Room 버튼 활성화
        joinRoom.interactable = true;                       // Join Room 버튼 활성화
    }

    private string GetUserID()                          // 사용자 ID를 생성하는 함수
    {
        string userID = PlayerPrefs.GetString("USER_ID");
        if (string.IsNullOrEmpty(userID))
        {
            userID = "USER_" + Random.Range(0, 999).ToString("000");
        }
        return userID;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)       // 빈 방이 없어 입장에 실패했을 때 호출되는 콜백함수
    {
        Debug.Log("Online : 새로운 방을 생성합니다.");
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 20 });
    }

    public override void OnJoinedRoom()                                             // 방에 입장했을 때 호출되는 콜백함수
    {
        Debug.Log("Online : 방에 입장했습니다.");
        StartCoroutine(LoadSceneLoader());
    }

    private IEnumerator LoadSceneLoader()
    {
        PhotonNetwork.IsMessageQueueRunning = false;                        // 메시지 큐를 중지. Scene이 이동하는 동안 메시지 큐를 중지시키기 위함
        AsyncOperation ao = SceneManager.LoadSceneAsync("SceneLoader");     // SceneLoader를 비동기로 로드. 비동기는 다른 작업을 하면서 로드가 가능
        yield return ao;
    }

    public void OnClickJoinRandomRoom()                                             // Join Random Room 버튼 클릭 시 호출되는 함수
    {
        joinRandomRoom.interactable = false;
        joinRoom.interactable = false;
        PhotonNetwork.NickName = userID.text;
        PlayerPrefs.SetString("USER_ID", userID.text);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickCreateRoom()
    {
        joinRandomRoom.interactable = false;
        joinRoom.interactable = false;
        string roomname = roomName.text;
        if (string.IsNullOrEmpty(roomName.text))
            roomname = "Room_" + Random.Range(0, 999).ToString("000");
        PhotonNetwork.NickName = userID.text;
        PlayerPrefs.SetString("USER_ID", userID.text);

        RoomOptions roomOptions = new();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomname, roomOptions, TypedLobby.Default);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RoomItem"))
        {
            Destroy(obj);
        }
        foreach (RoomInfo roomInfo in roomList)
        {
            GameObject room = Instantiate(roomItem);                    // Room Item object 생성
            room.transform.SetParent(scrollContent.transform, false);   // Room Item object를 Content object의 자식으로 설정

            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = roomInfo.Name;
            roomData.connectPlayer = roomInfo.PlayerCount;
            roomData.maxPlayers = roomInfo.MaxPlayers;
            roomData.DisplayRoomData();
            roomData.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); });

            if (roomData.connectPlayer == 0)
                Destroy(room);
        }
    }
    internal void OnClickRoomItem(string roomName)
    {
        PhotonNetwork.NickName = userID.text;
        PlayerPrefs.SetString("USER_ID", userID.text);

        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed");
        joinRandomRoom.interactable = true;
        joinRoom.interactable = true;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed");
        joinRandomRoom.interactable = true;
        joinRoom.interactable = true;
    }















    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.InRoom.ToString());
    }

}
