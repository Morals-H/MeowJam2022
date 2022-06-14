using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Collectable_Master : MonoBehaviour
{
    public PhotonView View;
    public List<Collectable> Table;
    public string[] Collected;
    public Text YYCounter;

    // Start is called before the first frame update
    void Start()
    {
        View = GetComponent<PhotonView>();
        ReloadArray();
    }

    private void FixedUpdate()
    {
        YYCounter.text =  "" + Collected.Length;
    }
    public void ReloadArray()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Collected = PlayerPrefsX.GetStringArray("Collected");
            View.RequestOwnership();
            if (Collected.Length > 1)
            {
                //checking if has been collected
                View.RPC("RPC_ColSync", RpcTarget.All, Collected);
            }
            else if (Collected.Length == 1)
            {
                //checking if has been collected
                View.RPC("RPC_ColSyncSingle", RpcTarget.All, Collected);
            }
        }
        else
        {
            View.RequestOwnership();
            View.RPC("RPC_ColSyncReq", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void RPC_ColSyncReq()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ReloadArray();
        }
    }

    //syncing broken objects
    [PunRPC]
    public void RPC_ColSyncSingle(string ColTags)
    {
        Collected = new string[1];
        Collected[0] = ColTags;
        for (int i2 = 0; i2 < (Table.Capacity); i2++)
        {
            if (ColTags == Table[i2].ColTag && Table[i2])
            {
                if (Table[i2].Timer == -1) Destroy(Table[i2].gameObject);
            }

        }
    }

    //syncing broken objects
    [PunRPC]
    public void RPC_ColSync(string[] ColTags)
    {
        Collected = ColTags;
        for (int i = 0; i < (ColTags.Length); i++)
        {
            for (int i2 = 0; i2 < (Table.Capacity); i2++)
            {
                if (ColTags[i] == Table[i2].ColTag && Table[i2])
                {
                    if(Table[i2].Timer == -1) Destroy(Table[i2].gameObject);
                }
            }
        }
    }

}
