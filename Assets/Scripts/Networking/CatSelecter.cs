using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CatSelecter : MonoBehaviour
{
    public GameObject Ink, Yuki;
    private PhotonView view;
    private int timer = 25;

    // Start is called before the first frame update
    void Start()
    {
        Ink = GameObject.Find("Ink");
        Yuki = GameObject.Find("Yuki");
        view = GetComponent<PhotonView>();
        view.RequestOwnership();
    }

    private void FixedUpdate()
    {
        if (timer > 0) timer--;
        if (timer == 1) Joined();
    }

    //if no rpc call recieved
    void Joined()
    {
        if (Ink.GetComponent<PhotonView>().IsMine && Yuki.GetComponent<PhotonView>().IsMine)
        {
            //requesting player
            Ink.GetComponent<Player_Motor>().isPlayer = true;
            Ink.GetComponent<PhotonView>().RequestOwnership();
            view.RPC("RPC_myCat", RpcTarget.OthersBuffered, "NotPlayable");
            //setting camera
            CinemachineFreeLook TPSCam = GameObject.Find("TPSCam").GetComponent<CinemachineFreeLook>();
            TPSCam.LookAt = Ink.transform.Find("Scarf").transform;
            TPSCam.Follow = Ink.transform;
            //setting canvas name
            GameObject.Find("Canvas").GetComponent<Master>().Cat = "Ink";
        }
    }

    //called to figure out whose cat is whose
    [PunRPC]
    public void RPC_myCat(string InkPlayable)
    {
        if (InkPlayable == "Playable")
        {
            //requesting player
            Ink.GetComponent<Player_Motor>().isPlayer = true;
            Ink.GetComponent<PhotonView>().RequestOwnership();
            Ink.GetComponent<AudioSource>().Play();
            view.RPC("RPC_myCat", RpcTarget.OthersBuffered, "NotPlayable");
            //setting camera
            CinemachineFreeLook TPSCam = GameObject.Find("TPSCam").GetComponent<CinemachineFreeLook>();
            TPSCam.LookAt = Ink.transform.Find("Scarf").transform;
            TPSCam.Follow = Ink.transform;
            //setting canvas name
            GameObject.Find("Canvas").GetComponent<Master>().Cat = "Ink";
        }
        else
        {
            //requesting player
            Yuki.GetComponent<Player_Motor>().isPlayer = true;
            Yuki.GetComponent<PhotonView>().RequestOwnership();
            Yuki.GetComponent<AudioSource>().Play();
            view.RPC("RPC_myCat", RpcTarget.OthersBuffered, "Playable");
            //setting camera
            CinemachineFreeLook TPSCam = GameObject.Find("TPSCam").GetComponent<CinemachineFreeLook>();
            TPSCam.LookAt = Yuki.transform.Find("Scarf").transform;
            TPSCam.Follow = Yuki.transform;
            //setting canvas name
            GameObject.Find("Canvas").GetComponent<Master>().Cat = "Yuki";
        }
    }
}