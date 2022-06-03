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
    private PhotonView View;

    // Start is called before the first frame update
    void Start()
    {
        View = GetComponent<PhotonView>();

        //hiding cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //preparing networking
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;
    }

    //getting our cat
    [PunRPC]
    public void RPC_RequestCat()
    {
        View.RPC("RPC_MyCat", RpcTarget.All, gameObject.name);
    }

    [PunRPC]
    public void RPC_MyCat(string OtherCat)
    {
        Cat = OtherCat;

        //setting camera
        CinemachineFreeLook TPSCam = GameObject.Find("TPSCam").GetComponent<CinemachineFreeLook>();

        //the reverse 
        if (Cat == "yuki")
        {
            Ink.GetComponent<Player_Motor>().isPlayer = true;
            TPSCam.LookAt = Ink.transform.Find("Scarf").transform;
            TPSCam.Follow = Ink.transform;

        }
        else
        {
            Yuki.GetComponent<Player_Motor>().isPlayer = true;
            TPSCam.LookAt = Yuki.transform.Find("Scarf").transform;
            TPSCam.Follow = Yuki.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Ink.GetComponent<PhotonView>().IsMine && !Yuki.GetComponent<PhotonView>().IsMine)
        {
            View.RPC("RPC_RequestCat", RpcTarget.All);
        }

        if (Input.GetAxis("Pause") > 0 && !SetPause)
        {
            SetPause = true;
            if (PauseMenu.activeSelf)
            {
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
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
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
}
