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
            //updating player prefs
            List<string> Collected = new List<string>(PlayerPrefsX.GetStringArray("Collected"));
            Collected.Add(tag);
            PlayerPrefsX.SetStringArray("Collected", Collected.ToArray());

            //effect
            Timer = 100;
            transform.Find("YingYang_LOD0").GetComponent<MeshRenderer>().enabled = false;
            transform.Find("YingYang_LOD1").GetComponent<MeshRenderer>().enabled = false;
            GetComponent<AudioSource>().Play();
        }
    }

    //recieving a request to destroy self
    [PunRPC]
    public void RPC_Break(int Player, string Weapon)
    {
        //updating player prefs
        List<string> Collected = new List<string>(PlayerPrefsX.GetStringArray("Collected"));
        Collected.Add(tag);
        PlayerPrefsX.SetStringArray("Collected", Collected.ToArray());

        //effect
        Timer = 100;
        View.RPC("RPC_Break", RpcTarget.AllBuffered, View.ViewID);
        GetComponent<MeshRenderer>().enabled = false;
    }

}
