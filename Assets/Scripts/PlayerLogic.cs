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
    public int startBPM = 60;
    public int endBPM = 220;
    float bpmCoefficient;
    public float bps = 1;
    float nextHeartbeat;
    Health health;

    private AudioListener timeAudio;
    private PlayerSoundEffect soundEffects;
    private bool tracking = false;
    private Vector3 position;

    public GoalReachedEvent notifyFinished;
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        health = GetComponent<Health>();
        soundEffects = GetComponent<PlayerSoundEffect>();

        bpmCoefficient = (endBPM - startBPM) / Mathf.Pow(health.maxHealth, 2);
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
            soundEffects.pitchBackgroundMusic(1f);
        }
        else
        {
            soundEffects.pitchBackgroundMusic(Mathf.Max(.6f, distance * distance_factor));
        }
        startTracking();
    }

    void startTracking()
    {

        if (gameObject.activeSelf || true)
        {
            position = gameObject.transform.position;
            Invoke("trackActivity", 0.2f);
        }
    }

    void onDisable()
    {
        soundEffects.stopBackgroundMusic();
    }

    public void ResetPlayer(){
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


        if (health.healthPoints > 0 && health.healthPoints <= 2 * health.maxHealth / 3)
        {
            if (nextHeartbeat > bps)
            {
                float bpm = bpmCoefficient * Mathf.Pow(health.healthPoints - health.maxHealth, 2) + startBPM;
                bps = 60f / bpm;
                //audioSource.PlayOneShot(heartbeatClip);
                nextHeartbeat = 0;
            }
            else
            {
                nextHeartbeat += Time.deltaTime;
            }
        }
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
