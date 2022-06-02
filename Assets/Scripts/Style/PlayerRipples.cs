using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRipples : MonoBehaviour
{
    private Transform Player;
    public string Target;
    private Vector3 Home;

    private void Start()
    {
        Home = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player) transform.position = new Vector3(Player.position.x, transform.position.y, Player.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Player && other.name == Target)
        {
            GetComponent<MeshRenderer>().enabled = true;
            Player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == Target)
        {
            GetComponent<MeshRenderer>().enabled = false;
            Player = null;
            transform.position = Home;
        }
    }
}
