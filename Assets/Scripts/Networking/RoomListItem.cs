using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public Menu MM;
    public Text Name;
    public Text Players;
    public Text Ping;
    RoomInfo info;

    public void Start()
    {
        MM = GameObject.Find("Canvas").GetComponent<Menu>();
    }

    public void SetUp(RoomInfo _Info)
    {
        info = _Info;
        Name.text = info.Name;
        Ping.text = PhotonNetwork.GetPing() + "ms"; 
        Players.text = _Info.PlayerCount + "/" + _Info.MaxPlayers;
    }

    //When button is clicked
    public void OnClick()
    {
        if (info.PlayerCount < info.MaxPlayers) 
        {
            PhotonNetwork.JoinRoom(Name.text);
        }
        else
        {
            MM.Load_Text.GetComponent<Text>().text = "Error: Room full";
        }
    }
}
