using UnityEngine;
using SpeechIO;
public class PlayerSoundEffect : MonoBehaviour
{
    private AudioSource backgroundSource;
    public AudioClip finishSound;
    public GameObject Goal;
    public AudioClip backgroundClip;
    private bool backgroundClipIsActive = false;

    float backgroundVolume = 0.2f;

    public void ResetMusic()
    {
        backgroundSource = Goal.GetComponent<AudioSource>();
        backgroundSource.volume = backgroundVolume;
        backgroundSource.clip = backgroundClip;
        backgroundClipIsActive = false;
    }

    public void playFinisherClip()
    {
        backgroundSource.volume = 0.8f;
        backgroundSource.clip = finishSound;
        backgroundSource.pitch = 1;
        backgroundSource.Play();
    }


    public void pitchBackgroundMusic(float pitchValue)
    {
        //backgroundSource.pitch = pitchValue > 1 ? 1 : pitchValue;
        backgroundSource.pitch = pitchValue;
    }

    public void startBackgroundMusic()
    {
        backgroundSource.Play();
        backgroundSource.volume = backgroundVolume;
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