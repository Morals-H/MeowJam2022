using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SceneTransition : MonoBehaviour
{
    private GameObject Char;

    //internal
    public GameObject Text;
    public string Scene;
    public string NextSpawn;
    private bool Active;

    // Update is called once per frame
    void Update()
    {
        if (Char && Input.GetAxis("Interact") > 0 && Char.GetComponent<PhotonView>().IsMine && !Active)
        {
            Active = true;
            PlayerPrefs.SetString("Spawn", NextSpawn);
            GameObject.Find("Canvas").GetComponent<Master>().LoadScreen.SetActive(true);
            PhotonNetwork.Destroy(Char);
            PhotonNetwork.LoadLevel(Scene);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine)
        {
            Char = other.gameObject;
            Text.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Char)
        {
            Char = null;
            Text.SetActive(false);
        }
    }
}
