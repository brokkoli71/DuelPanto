using UnityEngine;
using SpeechIO;
public class PlayerSoundEffect : MonoBehaviour
{
    public AudioSource backgroundSource;

    public AudioClip backgroundClip;
    private bool backgroundClipIsActive = false;
    void Start()
    {
        print("PlayerSoundEffect");
        backgroundSource = GetComponent<AudioSource>();
        backgroundSource.clip = backgroundClip;
    }

    public void pitchBackgroundMusic(float pitchValue)
    {
        print("want to pitch the music");
        //backgroundSource.mute = true;
        //backgroundSource.pitch = pitchValue;

    }

    public void startBackgroundMusic()
    {
        print("startt m,ucis");
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

    public void PlayClipPitched(AudioClip clip, float minPitch, float maxPitch)
    {
        // little trick to make clip sound less redundant
        backgroundSource.pitch = Random.Range(minPitch, maxPitch);
        // plays same clip only once, this way no overlapping
        backgroundSource.PlayOneShot(clip);
        backgroundSource.pitch = 1f;
    }

}