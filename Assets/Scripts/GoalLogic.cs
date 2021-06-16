using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DualPantoFramework;

public class GoalLogic : MonoBehaviour
{

    public AudioClip finalSound;
    private AudioSource _audioSource;
    // Use this for initialization
    void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        _audioSource.PlayOneShot(finalSound);
    }
}
