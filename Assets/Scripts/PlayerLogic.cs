using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
public class PlayerLogic : MonoBehaviour
{
    private PantoHandle upperHandle;

    AudioSource audioSource;
    public AudioClip heartbeatClip;

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

    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
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
    }

    void trackActivity()
    {
        float distance = Vector3.Distance(position, gameObject.transform.position);
        //print($"Activity: {distance}");
        if (distance != 0)
        {
            soundEffects.pitchBackgroundMusic(1f);
        }
        else
        {
            soundEffects.pitchBackgroundMusic(0.2f);
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

    void Update()
    {
        // Simply connects the player to the upper handles position
        transform.position = upperHandle.HandlePosition(transform.position);

        if (health.healthPoints > 0 && health.healthPoints <= 2 * health.maxHealth / 3)
        {
            if (nextHeartbeat > bps)
            {
                float bpm = bpmCoefficient * Mathf.Pow(health.healthPoints - health.maxHealth, 2) + startBPM;
                bps = 60f / bpm;
                audioSource.PlayOneShot(heartbeatClip);
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
        print("triggerd something");
        if (other.CompareTag("Goal"))
        {
            print("You reached the goal!");
            soundEffects.stopBackgroundMusic();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        print("collided with something");
        if (other.gameObject.CompareTag("Goal"))
        {
            print("You reached the goal!");
            soundEffects.stopBackgroundMusic();
        }


    }
}
