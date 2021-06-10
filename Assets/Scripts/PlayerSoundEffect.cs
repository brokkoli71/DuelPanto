﻿using UnityEngine;
using SpeechIO;
public class PlayerSoundEffect : MonoBehaviour
{
    public AudioClip dropInClip;
    public AudioClip gameOverClip;
    public AudioClip collisionClip;

    public AudioClip backgroundMusic;
    public float maxPitch = 1.2f;
    public float minPitch = 0.8f;
    private GameObject previousEnemy;
    private AudioSource audioSource;
    public SpeechOut speechOut = new SpeechOut();

    private bool bgMusic;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public float PlayerFellDown()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(gameOverClip);
        return gameOverClip.length;
    }

    public void startBackgroundMusic()
    {
        bgMusic = true;
        audioSource.PlayOneShot(backgroundMusic);
    }

    public void pitchBackgroundMusic(float pitchValue)
    {
        audioSource.pitch = pitchValue;

    }
    public bool IsBackgroundMusicActive()
    {
        return bgMusic;
    }

    public void stopBackgroundMusic()
    {
        bgMusic = false;
    }

    public void pitchMusic(AudioSource music)
    {

    }
    public void PlayHit()
    {
        PlayClipPitched(collisionClip, minPitch, maxPitch);
    }
    public void PlayDropIn()
    {
        audioSource.PlayOneShot(dropInClip);
    }

    public void StopPlayback()
    {
        audioSource.Stop();
    }

    public void PlayClipPitched(AudioClip clip, float minPitch, float maxPitch)
    {
        // little trick to make clip sound less redundant
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        // plays same clip only once, this way no overlapping
        audioSource.PlayOneShot(clip);
        audioSource.pitch = 1f;
    }

}