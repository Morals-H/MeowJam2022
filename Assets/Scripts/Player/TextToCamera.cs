using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToCamera : MonoBehaviour
{
    private Transform TPSCam;
    private TextMesh Text;
    public int Timer;

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMesh>();
        TPSCam = GameObject.Find("TPSCam").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Timer > 0) Timer--;
        else if (Text.text != "")
        {
            Text.text = Text.text.Substring(1);
            Timer = 20;
        }

        transform.LookAt(TPSCam);
    }
}
