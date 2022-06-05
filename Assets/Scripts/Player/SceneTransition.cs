using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SceneTransition : MonoBehaviour
{
    private GameObject Ink, Yuki;
    public string Scene;
    public GameObject Text;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Interact") > 0 && Ink && Yuki)
        {
            PhotonNetwork.LoadLevel(Scene);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Yuki")
        {
            Yuki = other.gameObject;
            Text.SetActive(true);
        }
        else if (other.name == "Ink")
        {
            Ink = other.gameObject;
            Text.SetActive(true);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Yuki")
        {
            Yuki = null;
            Text.SetActive(false);
        }
        else if (other.name == "Ink")
        {
            Ink = null;
            Text.SetActive(false);
        }
    }
}
