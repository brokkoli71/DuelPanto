using UnityEngine;
using SpeechIO;
public class PlayerSoundEffect : MonoBehaviour
{
    public AudioClip dropInClip;
    public AudioClip gameOverClip;
    public AudioClip collisionClip;

    public float maxPitch = 1.2f;
    public float minPitch = 0.8f;
    private GameObject previousEnemy;
    public AudioSource backgroundSource;
    public SpeechOut speechOut = new SpeechOut();

    private bool bgMusic;
    void Start()
    {
        backgroundSource = GetComponent<AudioSource>();
    }



    public void pitchBackgroundMusic(float pitchValue)

    {

        print("want to pitch the music");
        backgroundSource.mute = true;
        //backgroundSource.pitch = pitchValue;

    }

    public void startBackgroundMusic()
    {
        bgMusic = true;
        //backgroundSource.Play();
    }
    public bool IsBackgroundMusicActive()
    {
        return bgMusic;
    }

    public void stopBackgroundMusic()
    {
        bgMusic = false;
        backgroundSource.Stop();
    }

    public void PlayHit()
    {
        PlayClipPitched(collisionClip, minPitch, maxPitch);
    }
    public void PlayDropIn()
    {
        backgroundSource.PlayOneShot(dropInClip);
    }

    public void StopPlayback()
    {
        backgroundSource.Stop();
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