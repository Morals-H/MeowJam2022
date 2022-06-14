using UnityEngine;

public class HidePlayer : MonoBehaviour
{
    public GameObject Yuki_Scarf, Ink_Scarf;
    public GameObject Yuki_Skin, Ink_Skin;

    private void Start()
    {
        //Yuki = GameObject.Find("Yuki").transform.Find("Scarf").gameObject;
        //Ink = GameObject.Find("Ink").transform.Find("Scarf").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Yuki_Scarf) 
        {
            //yuki
            if (Vector3.Distance(transform.position, Yuki_Scarf.transform.position) < 1.5)
            {
                Yuki_Scarf.GetComponent<SkinnedMeshRenderer>().enabled = false;
                Yuki_Skin.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
            else
            {
                Yuki_Scarf.GetComponent<SkinnedMeshRenderer>().enabled = true;
                Yuki_Skin.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            }
        }
        if (Ink_Scarf)
        {
            //ink
            if (Vector3.Distance(transform.position, Ink_Scarf.transform.position) < 1.5)
            {
                Ink_Scarf.GetComponent<SkinnedMeshRenderer>().enabled = false;
                Ink_Skin.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
            else
            {
                Ink_Scarf.GetComponent<SkinnedMeshRenderer>().enabled = true;
                Ink_Skin.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            }
        }
    }
}
