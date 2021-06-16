using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DualPantoFramework;

[System.Serializable]
public class GoalReachedEvent : UnityEvent<GameObject> { }

public class PlayerLogic : MonoBehaviour
{
    private PantoHandle upperHandle;

    //AudioSource audioSource;
    //public AudioClip heartbeatClip;

    public GameObject listener;
    private AudioListener timeAudio;
    private PlayerSoundEffect soundEffects;
    private bool tracking = false;

    public bool isPitched = false;
    private Vector3 position;

    public GoalReachedEvent notifyFinished;
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        soundEffects = GetComponent<PlayerSoundEffect>();
    }

    void FixedUpdate()
    {
        if (!tracking)
        {
            tracking = true;
            startTracking();
        }
        listener.transform.position = gameObject.transform.position;
    }


    void trackActivity()
    {
        float distance = Vector3.Distance(position, gameObject.transform.position);
        float distance_factor = 3.5f;
        //print($"Activity: {distance}");

        if (distance * distance_factor > 1)
        {
            isPitched = false;
            soundEffects.pitchBackgroundMusic(1f);
        }
        else
        {
            isPitched = true;
            soundEffects.pitchBackgroundMusic(Mathf.Max(.6f, distance * distance_factor));
        }
        startTracking();
    }

    void startTracking()
    {

        if (gameObject.activeSelf && GameObject.Find("Panto").GetComponent<GameManager>().gameRunning)
        {
            position = gameObject.transform.position;
            Invoke("trackActivity", 0.2f);
        }
    }

    void onDisable()
    {
        soundEffects.stopBackgroundMusic();
    }

    public void ResetPlayer()
    {
        soundEffects.ResetMusic();
    }

    void Update()
    {
        if (gameObject.activeSelf && !soundEffects.isBackgroundMusicActive())
        {
            soundEffects.startBackgroundMusic();
        }


        // Simply connects the player to the upper handles position
        transform.position = upperHandle.HandlePosition(transform.position);
        transform.rotation = Quaternion.AngleAxis(upperHandle.GetRotation(), Vector3.up);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            soundEffects.stopBackgroundMusic();
            soundEffects.playFinisherClip();
            notifyFinished.Invoke(gameObject);
        }
    }
}
