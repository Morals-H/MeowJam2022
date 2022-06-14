using UnityEngine;
using Cinemachine;

public class CameraAdjuster : MonoBehaviour
{
    private CinemachineFreeLook CM;
    private Vector3 Sensitivty;
    private bool X, Y;
    // Start is called before the first frame update
    void Start()
    {
        CM = GetComponent<CinemachineFreeLook>();
        if (PlayerPrefsX.GetVector3("Sensitivty").z == 1) RefreshSettings();
    }
    void RefreshSettings()
    {
        //loading
        Sensitivty = PlayerPrefsX.GetVector3("Sensitivty");
        X = PlayerPrefsX.GetBool("X");
        Y = PlayerPrefsX.GetBool("Y");

        //loading camera settings
        CM.m_XAxis.m_MaxSpeed = Sensitivty.x;
        CM.m_XAxis.m_InvertInput = X;
        CM.m_YAxis.m_MaxSpeed = Sensitivty.y;
        CM.m_YAxis.m_InvertInput = Y;
    }
}
