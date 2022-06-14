using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Menu : MonoBehaviourPunCallbacks
{
    //Networking GUi
    public InputField ServerHost;
    public InputField ServerJoin;
    public GameObject PlayMenu;
    private List<RoomInfo> rooms;
    private bool Con;

    //Server Listing
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform roomListContent;

    //Debugging
    public Text Load_Text;


    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("Spawn", "Spawn_Home");
        Load_Text.text = "Connecting...";
        PhotonNetwork.ConnectUsingSettings();
    }

    //when the connection to the master is completed
    public override void OnConnectedToMaster()
    {
        if (PlayerPrefs.GetString("Server") != PhotonNetwork.CloudRegion && PlayerPrefs.GetString("Server") != "")
        {
            if (PlayerPrefs.GetString("Server") == "US") US();
            else if (PlayerPrefs.GetString("Server") == "EU") EU();
        }

        Load_Text.text = "Connected";
        Con = true;
        PhotonNetwork.JoinLobby();
    }

    //when the connection to the lobby is complete
    public override void OnJoinedLobby()
    {
    }


    //when a room update has been recieved 
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        rooms = roomList;

        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    //when called to host
    void Host()
    {
        Load_Text.GetComponentInChildren<Text>().text = "Checking Name...";
        if (rooms.Count > 0)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].Name == ServerHost.text)
                {
                    Load_Text.text = "Failure.";
                    break;
                }
                else Hosting();
            }
        }
        else Hosting();
    }

    //when server hosting begins
    void Hosting()
    {
        //resetting stats
        PlayerPrefs.SetString("Spawn", "Spawn_Home");

        //loading game
        Load_Text.text = "Creating Game...";
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.CreateRoom(ServerHost.text, new RoomOptions { MaxPlayers = 2, IsVisible = true });
    }

    void Join()
    {
        //resetting 
        PlayerPrefsX.SetBool("FJ", true);
        PlayerPrefs.SetString("Spawn", "Spawn_Home");

        //loading game
        PlayerPrefsX.SetBool("FJ", true);
        Load_Text.text = "Joining Game...";
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(ServerJoin.text);
    }
    //When you enter a match
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("CamdanValley");
    }

    void Play()
    {
        if (Con)
        {
            if (PlayMenu.activeSelf == true) PlayMenu.SetActive(false);
            else PlayMenu.SetActive(true);
        }

    }
        //exiting the game
        void Exit()
    {
        PhotonNetwork.LeaveLobby();
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        try
        {
            PhotonNetwork.LeaveLobby();
        }
        catch
        {

        }
    }

    //deleting player prefs
    void Delete()
    {
        Load_Text.text = "Save Deleted";

        Vector3 Sensitivty = PlayerPrefsX.GetVector3("Sensitivty");
        Vector3 Sound = PlayerPrefsX.GetVector3("Sound");
        bool X = PlayerPrefsX.GetBool("X");
        bool Y = PlayerPrefsX.GetBool("Y");

        PlayerPrefs.DeleteAll();
        if (Sensitivty.z != 1)
        {
            Sound = new Vector3(1, 1, 1);
            Sensitivty = new Vector3(300, 5, 1);
            X = false;
            Y = true;
        }

        PlayerPrefsX.SetVector3("Sound", Sound);
        PlayerPrefsX.SetVector3("Sensitivty", Sensitivty);
        PlayerPrefsX.SetBool("X", X);
        PlayerPrefsX.SetBool("Y", Y);
    }


    //switching servers
    void US()
    {
        PlayerPrefs.GetString("Server", "US");

        if (Con == true)
        {
            Load_Text.text = "Setting Server US";

            Con = false;

            PhotonNetwork.Disconnect();

            PhotonNetwork.ConnectToRegion("us");

        }
        else
        {
            Load_Text.text = "Switching Servers To Fast";
        }
    }

    //switching servers
    void EU()
    {
        PlayerPrefs.GetString("Server", "EU");

        if (Con == true)
        {
            Load_Text.text = "Setting Server EU";

            Con = false;

            PhotonNetwork.Disconnect();

            PhotonNetwork.ConnectToRegion("EU");
        }
        else
        {
            Load_Text.text = "Switching Servers To Fast";
        }
    }

}
