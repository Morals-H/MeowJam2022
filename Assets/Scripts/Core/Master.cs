using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;

public class Master : MonoBehaviourPunCallbacks
{
    //pause logic
    public GameObject PauseMenu;
    private bool SetPause;
    public GameObject LoadScreen;

    //Chat
    private PhotonView view;
    public InputField Chat;

    //player logic
    public GameObject OurCat;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();

        //hiding cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //preparing networking
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Pause") > 0 && !SetPause)
        {
            SetPause = true;
            if (PauseMenu.activeSelf)
            {
                GameObject.Find("TPSCam").GetComponent<CinemachineFreeLook>().enabled = true;
                Resume();
            }
            else
            {
                PauseMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;

                OurCat.GetComponent<Animator>().SetFloat("Speed", 0);
                OurCat.GetComponent<Player_Motor>().enabled = false;

                GameObject.Find("TPSCam").GetComponent<CinemachineFreeLook>().enabled = false;
            }
        }
        else if(Input.GetAxis("Pause") == 0) SetPause = false;
    }

    //gui buttons
    //resuming game
    void Resume()
    {
        PauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameObject.Find("TPSCam").GetComponent<CinemachineFreeLook>().enabled = true;

        if (Chat.text != "")
        {
            //limiting chat digits
            if (Chat.text.Length > 30) Chat.text = Chat.text.Substring(0, 30);

            view.RequestOwnership();
            view.RPC("RPC_Chat", RpcTarget.All, OurCat.name, Chat.text);

            //resetting text
            Chat.text = "";

            OurCat.GetComponent<AudioSource>().Play();
        }

        //restoring player movement
        OurCat.GetComponent<Player_Motor>().enabled = true;
    }


    //recieving a message
    [PunRPC]
    public void RPC_Chat(string SenderCat, string Msg)
    {
        GameObject ChatTarget = OurCat;

        if (SenderCat == OurCat.name)
        {
            ChatTarget = OurCat;
        }
        else if (GameObject.Find("Ink_Cat(Clone)"))
        {
            ChatTarget = GameObject.Find("Ink_Cat(Clone)");
        }
        else if (GameObject.Find("Yuki_Cat(Clone)"))
        {
            ChatTarget = GameObject.Find("Yuki_Cat(Clone)");
        }
        ChatTarget.GetComponent<Player_Motor>().SendMessage("Chat", Msg);
    }

    void Menu()
    {
        LoadScreen.SetActive(true);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }

    private void OnApplicationQuit()
    {
        try
        {
            OurCat.GetComponent<Player_Motor>().isPlayer = false;

            PhotonNetwork.LeaveLobby();
        }
        catch
        {

        }
    }
}
