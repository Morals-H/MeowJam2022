using UnityEngine;

public class HidePlayer : MonoBehaviour
{
    private GameObject Yuki, Ink;

    private void Start()
    {
        Yuki = GameObject.Find("Yuki").transform.Find("Scarf").gameObject;
        Ink = GameObject.Find("Ink").transform.Find("Scarf").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //yuki
        if (Vector3.Distance(transform.position, Yuki.transform.position) < 1.5)
        {
            Yuki.GetComponent<SkinnedMeshRenderer>().enabled = false;
            Yuki.transform.root.Find("Yuki").GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }
        else
        {
            Yuki.GetComponent<SkinnedMeshRenderer>().enabled = true;
            Yuki.transform.root.Find("Yuki").GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        }

        //ink
        if (Vector3.Distance(transform.position, Ink.transform.position) < 1.5)
        {
            Ink.GetComponent<SkinnedMeshRenderer>().enabled = false;
            Ink.transform.root.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }
        else
        {
            Ink.GetComponent<SkinnedMeshRenderer>().enabled = true;
            Ink.transform.root.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        }
    }
}
