using UnityEngine;
using SpeechIO;
public class PlayerSoundEffect : MonoBehaviour
{
    private AudioSource backgroundSource;
    public GameObject Goal;
    public AudioClip backgroundClip;
    private bool backgroundClipIsActive = false;
    void Start()
    {
        backgroundSource = Goal.GetComponent<AudioSource>();
        print(backgroundSource);
        backgroundSource.clip = backgroundClip;
    }

    public void pitchBackgroundMusic(float pitchValue)
    {
        backgroundSource.pitch = pitchValue;
    }

    public void startBackgroundMusic()
    {
        backgroundSource.Play();
        backgroundClipIsActive = true;

    }
    public bool isBackgroundMusicActive()
    {
        return backgroundClipIsActive;
    }


    public void stopBackgroundMusic()
    {
        backgroundSource.Stop();
        backgroundClipIsActive = false;
    }
}