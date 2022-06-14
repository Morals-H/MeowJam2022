using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Selecter : MonoBehaviour
{
    public GameObject Ink, Yuki;
    private GameObject Player;
    private PhotonView view;
    public GameObject Canv;
    public HidePlayer Cam;

    //spawn related
    private Vector3 spawn_Pos;
    private Quaternion spawn_Rot;

    // Start is called before the first frame update
    void Start()
    {

        spawn_Pos = GameObject.Find(PlayerPrefs.GetString("Spawn")).transform.position;
        spawn_Rot = GameObject.Find(PlayerPrefs.GetString("Spawn")).transform.rotation;

        //obtaining view
        view = GetComponent<PhotonView>();


        if (!view.IsMine)
        {
            view.RequestOwnership();
            view.RPC("RPC_myCat", RpcTarget.Others, "Request", SceneManager.GetActiveScene().name, PlayerPrefs.GetString("Spawn"));
        }
        else
        {
            view.RPC("RPC_myCat", RpcTarget.All, "Cat", SceneManager.GetActiveScene().name, PlayerPrefs.GetString("Spawn"));
        }
    }
    //loading cat
    [PunRPC]
    public void RPC_myCat(string Cat, string Scene, string Spawn)
    {
        if (Cat == "Request" && Canv.GetComponent<Master>().OurCat || Cat == "Cat" && Canv.GetComponent<Master>().OurCat)
        {
            view.RPC("RPC_myCat", RpcTarget.Others, Canv.GetComponent<Master>().OurCat.name, SceneManager.GetActiveScene().name, PlayerPrefs.GetString("Spawn"));
        } //if information is requested

        if (SceneManager.GetActiveScene().name != Scene && PlayerPrefsX.GetBool("FJ") == true)
        {
            PlayerPrefsX.SetBool("FJ", false);
            PlayerPrefs.SetString("Spawn", Spawn);
            PhotonNetwork.LoadLevel(Scene);
        } 
        



        if (Cat == "Ink_Cat" && !Player)
        {
            Player = PhotonNetwork.Instantiate(Yuki.name, spawn_Pos, spawn_Rot);
            Player.name = "Yuki_Cat";
            PlayerPrefs.SetString("Cat", "Yuki");
            Player.GetComponent<Rigidbody>().velocity = Vector3.zero;

            //setting hide player
            Cam.Yuki_Skin = Player.transform.Find("Scarf").gameObject;
            Cam.Yuki_Scarf = Player.transform.Find("YukiSkin").gameObject;

            if (!GameObject.Find("Ink_Cat(Clone)")) view.RPC("RPC_Refresh", RpcTarget.Others, SceneManager.GetActiveScene().name);
        } //creating Yuki
        if (Cat == "Yuki_Cat" && !Player || Cat == "Cat" && !Player)
        {
            Player = PhotonNetwork.Instantiate(Ink.name, spawn_Pos, spawn_Rot);
            Player.name = "Ink_Cat";
            PlayerPrefs.SetString("Cat", "Ink");
            Player.GetComponent<Rigidbody>().velocity = Vector3.zero;

            //setting hide player
            Cam.Ink_Skin = Player.transform.Find("Scarf").gameObject;
            Cam.Ink_Scarf = Player.transform.Find("InkSkin").gameObject;

            if (!GameObject.Find("Yuki_Cat(Clone)")) view.RPC("RPC_Refresh", RpcTarget.Others, SceneManager.GetActiveScene().name);
        } //creating ink

        Player.GetComponent<Player_Motor>().isPlayer = true;

        //setting camera
        Canv.GetComponent<Master>().LoadScreen.SetActive(false);
        CinemachineFreeLook TPSCam = GameObject.Find("TPSCam").GetComponent<CinemachineFreeLook>();
        TPSCam.LookAt = Player.transform.Find("Scarf").transform;
        TPSCam.Follow = Player.transform;
        Canv.GetComponent<Master>().OurCat = Player;

        if (Scene != SceneManager.GetActiveScene().name)
        {
            view.RPC("RPC_Remove", RpcTarget.Others);
        }
    }

    //in the event of a duplicate
    [PunRPC]
    public void RPC_Remove()
    {
        if (Canv.GetComponent<Master>().OurCat.name == "Ink_Cat")
        {
            Destroy(GameObject.Find("Yuki_Cat(Clone)"));
        }
        else Destroy(GameObject.Find("Ink_Cat(Clone)"));
    }

        //reloading other cat for host
        [PunRPC]
    public void RPC_Refresh(string Scene)
    {
        if (Scene == SceneManager.GetActiveScene().name)
        {
            GameObject C;
            string rFresh;

            if (Canv.GetComponent<Master>().OurCat.name == "Ink_Cat")
            {
                C = GameObject.Find("Ink_Cat");
                rFresh = "Yuki_Cat";
            }
            else
            {
                C = GameObject.Find("Yuki_Cat");
                rFresh = "Ink_Cat";
            }
            spawn_Pos = C.transform.position;
            spawn_Rot = C.transform.rotation;
            PhotonNetwork.Destroy(C);

            view.RequestOwnership();
            Player = null;
            RPC_myCat(rFresh, SceneManager.GetActiveScene().name, PlayerPrefs.GetString("Spawn"));
        }
    }
}
