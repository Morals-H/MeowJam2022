using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Collectable : MonoBehaviourPunCallbacks
{
    public int Timer = -1;
    public string ColTag;
    private PhotonView View;
    public List<string> Collected;

    void Awake()
    {
        View = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (Timer > 0) Timer--;
        else if (Timer == 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && Timer == -1)
        {
            //updating player prefs
            Collected = new List<string>(PlayerPrefsX.GetStringArray("Collected"));
            Collected.Add(ColTag);
            PlayerPrefsX.SetStringArray("Collected", Collected.ToArray());

            //effect
            Timer = 100;
            transform.Find("YingYang_LOD0").GetComponent<MeshRenderer>().enabled = false;
            transform.Find("YingYang_LOD1").GetComponent<MeshRenderer>().enabled = false;
            GetComponent<AudioSource>().Play();
            View.RPC("RPC_Break", RpcTarget.Others, tag);
            SendMessageUpwards("ReloadArray");
        }
    }

    //recieving a request to destroy self
    [PunRPC]
    public void RPC_Break(string RPCTag)
    {
        if (RPCTag == tag && Timer == -1)
        {
            //updating player prefs
            if (PhotonNetwork.IsMasterClient)
            {
                Collected = new List<string>(PlayerPrefsX.GetStringArray("Collected"));
                Collected.Add(ColTag);
                PlayerPrefsX.SetStringArray("Collected", Collected.ToArray());
            }

            //effect
            Timer = 100;
            transform.Find("YingYang_LOD0").GetComponent<MeshRenderer>().enabled = false;
            transform.Find("YingYang_LOD1").GetComponent<MeshRenderer>().enabled = false;
            GetComponent<AudioSource>().Play();
            SendMessageUpwards("ReloadArray");
        }
    }

}
