using UnityEngine;
using SpeechIO;
public class PlayerSoundEffect : MonoBehaviour
{
    private AudioSource backgroundSource;

    private AudioSource playerSource;
    public AudioClip finishSound;

    public AudioClip goalReachedSound;
    public GameObject Goal;
    public AudioClip backgroundClip;
    private bool backgroundClipIsActive = false;

    float backgroundVolume = 0.5f;

    public void ResetMusic()
    {
        backgroundSource = Goal.GetComponent<AudioSource>();
        playerSource = gameObject.GetComponent<AudioSource>();
        backgroundSource.volume = backgroundVolume;
        backgroundSource.clip = backgroundClip;
        playerSource.Stop();
        backgroundClipIsActive = false;
    }
  
    public void playerAudio(AudioClip clip, float volume){
        playerSource.PlayOneShot(clip,volume);
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