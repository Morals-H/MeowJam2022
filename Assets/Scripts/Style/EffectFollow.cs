using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EffectFollow : MonoBehaviour
{
    public GameObject Yuki, Ink;
    private Transform Local;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Local && Yuki.GetComponent<PhotonView>().IsMine)
        {
            Local = Yuki.transform;
        }
        else if (Ink.GetComponent<PhotonView>().IsMine)
        {
            Local = Ink.transform;
        }

        if(Local) transform.position = Local.position;
    }
}
