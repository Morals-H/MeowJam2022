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

    //Chat
    public InputField Chat;

    //player logic
    public GameObject Yuki, Ink;
    public string Cat;

    // Start is called before the first frame update
    void Start()
    {

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
                if (Cat == "Yuki")
                {
                    Yuki.GetComponent<Animator>().SetFloat("Speed", 0);
                    Yuki.GetComponent<Player_Motor>().enabled = false;
                }
                else
                {
                    Ink.GetComponent<Animator>().SetFloat("Speed", 0);
                    Ink.GetComponent<Player_Motor>().enabled = false;
                }
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

            //sending the actual message 
            if (Cat == "Yuki") Yuki.GetComponent<PhotonView>().RPC("RPC_Chat", RpcTarget.AllBuffered, Cat, Chat.text);
            else Ink.GetComponent<PhotonView>().RPC("RPC_Chat", RpcTarget.AllBuffered, Cat, Chat.text);

            //resetting text
            Chat.text = "";

            if (Cat == "Yuki") Yuki.GetComponent<AudioSource>().Play();
            else Ink.GetComponent<AudioSource>().Play();
        }

        //restoring player movement
        if (Cat == "Yuki") Yuki.GetComponent<Player_Motor>().enabled = true;
        else Ink.GetComponent<Player_Motor>().enabled = true;
    }

    void Menu()
    {
        PhotonNetwork.RemoveBufferedRPCs();

        if (Cat == "Yuki") Yuki.GetComponent<Player_Motor>().isPlayer = false;
        else Ink.GetComponent<Player_Motor>().isPlayer = false;
        
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (Cat == "Yuki") Yuki.GetComponent<Player_Motor>().isPlayer = false;
            else Ink.GetComponent<Player_Motor>().isPlayer = false;

            PhotonNetwork.LeaveLobby();
        }
        catch
        {

        }
    }
}
