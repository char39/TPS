using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;

public class RoomData : MonoBehaviourPun
{
    public Text textRoomName;
    public Text textConnectInfo;
    internal string roomName = string.Empty;
    internal int connectPlayer = 0;
    internal int maxPlayers = 0;

    public void DisplayRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/" + maxPlayers.ToString() + ")";
    }
}
