using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Collectable : MonoBehaviourPunCallbacks
{
    private int Timer = -1;
    public string Tag;
    private PhotonView View;

    void Awake()
    {
        View = GetComponent<PhotonView>();

        //checking if has been collected
        List<string> Collected = new List<string>(PlayerPrefsX.GetStringArray("Collected"));

        if (Collected.Contains(Tag + ","))
        {
            View.RPC("RPC_Break", RpcTarget.AllBuffered, View.ViewID);
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (Timer > 0) Timer--;
        else if (Timer == 0) Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && Timer == -1)
        {
            Timer = 150;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<AudioSource>().Play();
        }
    }

    //recieving a request to destroy self
    [PunRPC]
    public void RPC_Break(int Player, string Weapon)
    {
        Timer = 150;
        GetComponent<MeshRenderer>().enabled = false;
    }

}
