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

    public GameObject listener;

    public GameObject levelVoice;

    public bool goalReached = false;
    private PlayerSoundEffect soundEffects;

    private GameObject panto;
    private bool tracking = false;

    public bool isPitched = false;
    private Vector3 position;

    public GoalReachedEvent notifyFinished;
    void Start()
    {
        panto = GameObject.Find("Panto");
        upperHandle = panto.GetComponent<UpperHandle>();
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
        if (!goalReached)
        {
            float distance = Vector3.Distance(position, gameObject.transform.position);
            //print($"Activity: {distance}");

            if (distance > 0.05f)
            {
                isPitched = false;
                soundEffects.pitchBackgroundMusic(1f);
            }
            else
            {
                isPitched = true;
                soundEffects.pitchBackgroundMusic(0.5f);
                //soundEffects.pitchBackgroundMusic(Mathf.Max(.6f, distance * distance_factor));
            }
            startTracking();
        }
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

    public void ResetPlayer()
    {
        goalReached = false;
        tracking = false;
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
            if (panto.GetComponent<GameManager>().allEnemiesdefeated)
            {
                goalReached = true;
                soundEffects.stopBackgroundMusic();
                soundEffects.playFinisherClip();
                notifyFinished.Invoke(gameObject);
            }
        }
    }
}
