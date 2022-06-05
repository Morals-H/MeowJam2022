using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Image Cog;
    public Slider xSens, ySens, Audio, SFX, Music;
    public Toggle xInv, yInv;

    private Vector3 Sensitivty;
    private Vector3 Sound;
    private bool X, Y;

    private void Start()
    {
        Sound = PlayerPrefsX.GetVector3("Sound");
        Sensitivty = PlayerPrefsX.GetVector3("Sensitivty");
        X = PlayerPrefsX.GetBool("X");
        Y = PlayerPrefsX.GetBool("Y");


        if (Sensitivty.z == 1)
        {
            //sound related
            Audio.value = Sound.x;
            SFX.value = Sound.y;
            Music.value = Sound.z;

            //control related
            xSens.value = Sensitivty.x;
            ySens.value = Sensitivty.y;
            xInv.isOn = X;
            yInv.isOn = Y;
        }
        else if (Sensitivty.z != 1)
        {
            Sound = new Vector3(1, 1, 1);
            Sensitivty = new Vector3(300, 5, 1);
            X = false;
            Y = true;

            PlayerPrefsX.SetVector3("Sound", Sound);
            PlayerPrefsX.SetVector3("Sensitivty", Sensitivty);
            PlayerPrefsX.SetBool("X", X);
            PlayerPrefsX.SetBool("Y", Y);
        }
    }

    private void Update()
    {
        Cog.transform.Rotate(0,0,10*Time.deltaTime);
    }

    //sound related
    void Audio_Change()
    {
        Sound.x = Audio.value;
    }
    void SFX_Change()
    {
        Sound.y = SFX.value;
    }
    void Music_Change()
    {
        Sound.z = Music.value;
    }

    //control related
    void xInverted()
    {
        X = xInv.isOn;
    }
    void xSensitivty()
    {
        Sensitivty.x = xSens.value;
    }
    void yInverted()
    {
        Y = yInv.isOn;
    }
    void ySensitivty()
    {
        Sensitivty.y = ySens.value;
    }

    //saving settings
    void Save()
    {
        //save checker
        Sensitivty.z = 1;

        PlayerPrefsX.SetVector3("Sound", Sound);
        PlayerPrefsX.SetBool("X", X);
        PlayerPrefsX.SetBool("Y", Y);
        PlayerPrefsX.SetVector3("Sensitivty", Sensitivty);

        //sending audio updates
        GameObject[] Objs = GameObject.FindGameObjectsWithTag("Audio");
        foreach (GameObject i in Objs)
        {
            Debug.Log(i.name);
            i.SendMessage("RefreshSettings");
        }
        Objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject i in Objs) i.SendMessage("RefreshSettings");
        Objs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject i in Objs) i.SendMessage("RefreshSettings");

        //sending camera updates
        if (GameObject.Find("TPSCam"))
        {
            GameObject.Find("TPSCam").SendMessage("RefreshSettings");
        }

        //updating text
        if (GameObject.Find("Canvas").GetComponent<Menu>()) GameObject.Find("Canvas").GetComponent<Menu>().Load_Text.text = "Settings Saved";
    }

    //closing without saving settings
    void Close()
    {
        //closng pause menu
        if (GameObject.Find("TPSCam"))
        {
            GameObject.Find("Canvas").GetComponent<Master>().SendMessage("Resume");
        }

        Sound = PlayerPrefsX.GetVector3("Sound");
        Sensitivty = PlayerPrefsX.GetVector3("Sensitivty");
        X = PlayerPrefsX.GetBool("X");
        Y = PlayerPrefsX.GetBool("Y");

        //resetting the visuals
        Cog.transform.eulerAngles = new Vector3(0, 0, 0);

        //sound related
        Audio.value = Sound.x;
        SFX.value = Sound.y;
        Music.value = Sound.z;

        //control related
        xSens.value = Sensitivty.x;
        ySens.value = Sensitivty.y;
        xInv.isOn = X;
        yInv.isOn = Y;

        gameObject.SetActive(false);
    }
}
