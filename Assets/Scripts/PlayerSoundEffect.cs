using UnityEngine;
using SpeechIO;
public class PlayerSoundEffect : MonoBehaviour
{
    private AudioSource backgroundSource;

    public AudioClip[] collisionClip;
    public AudioClip lastCollisionClip;
    public float maxPitch = 1.2f;
    public float minPitch = 0.8f;
    public GameObject Goal;
    public AudioClip backgroundClip;
    public float audioCoolDown = 0.2f;
    private float lastCollision;
    private bool backgroundClipIsActive = false;
    void Start()
    {
        backgroundSource = Goal.GetComponent<AudioSource>();
        backgroundSource.clip = backgroundClip;
    }

    public AudioClip GetCollisionClip()
    {
        if (collisionClip.Length > 1)
        {
            AudioClip randomClip;
            do
            {
                randomClip = collisionClip[Random.Range(0, collisionClip.Length)];

            } while (randomClip == lastCollisionClip);
            lastCollisionClip = randomClip;
            return lastCollisionClip;
        }
        return collisionClip[0];
    }

    public void PlayHit()
    {
        if (Time.time < lastCollision + audioCoolDown)
            return;

        PlayClipPitched(GetCollisionClip(), minPitch, maxPitch);
        lastCollision = Time.time;
    }

    public void PlayClipPitched(AudioClip clip, float minPitch, float maxPitch)
    {
        // little trick to make clip sound less redundant
        backgroundSource.pitch = Random.Range(minPitch, maxPitch);
        // plays same clip only once, this way no overlapping
        backgroundSource.PlayOneShot(clip);
        backgroundSource.pitch = 1f;
    }
    public void pitchBackgroundMusic(float pitchValue)
    {
        backgroundSource.pitch = pitchValue > 1 ? 1 : pitchValue;
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