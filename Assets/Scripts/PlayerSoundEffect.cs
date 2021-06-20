using UnityEngine;
using SpeechIO;
public class PlayerSoundEffect : MonoBehaviour
{
    private AudioSource backgroundSource;
    public AudioClip finishSound;
    public float maxPitch = 1.2f;
    public float minPitch = 0.8f;
    public GameObject Goal;
    public AudioClip backgroundClip;
    private bool backgroundClipIsActive = false;


    public void ResetMusic()
    {
        backgroundSource = Goal.GetComponent<AudioSource>();
        backgroundSource.volume = 0.05f;
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
        backgroundSource.pitch = pitchValue > 1 ? 1 : 0.5f;
    }

    public void startBackgroundMusic()
    {
        backgroundSource.Play();
        backgroundSource.volume /= 3;
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