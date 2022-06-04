using UnityEngine;
public class AudioAdjuster : MonoBehaviour
{
    public string Type;
    public float DesiredAudio;

    // Start is called before the first frame update
    void Start()
    {
        if (DesiredAudio == 0) DesiredAudio = 1;
        if (PlayerPrefsX.GetVector3("Sensitivty").z == 1) RefreshSettings();
    }
    void RefreshSettings()
    {
        Vector3 Sound = PlayerPrefsX.GetVector3("Sound");
        if (Type == "Music")
        {
            GetComponent<AudioSource>().volume = Sound.z * Sound.x * DesiredAudio;
        }
        else
        {
            GetComponent<AudioSource>().volume = Sound.y * Sound.x * DesiredAudio;
        }
    }
}
