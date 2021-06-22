using UnityEngine;
using UnityEngine.AI;

public class EnemyLogic : MonoBehaviour
{
    public Transform target;
    public LayerMask layerMask;
    public float aimbotDistance = 10f;
    public float seekingDistance = 1f;
    public EnemyConfig config;
    public AudioClip[] foundPlayerClips;
    public AudioClip[] walkingClips;
    private AudioSource _audioSource;

    public bool foundPlayer = false;
    private bool walking = false;
    float lastWalked = 0f;
    private bool firstFinding = false;
    float timeToFind;

    private Vector3 spawnPosition;
    private GameObject player;
    private Quaternion spawnRotation;
    Vector3 lastSeenPosition;
    Vector3 oldLastPosition;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = config.CSGoPlayer ? aimbotDistance : seekingDistance;
        player = GameObject.Find("Player");
        _audioSource = gameObject.GetComponent<AudioSource>();
        // needed for walkingClips
        oldLastPosition = spawnPosition;
    }

    void OnEnable()
    {
        if (config.attackPlayerAtStart)
        {
            lastSeenPosition = target.position;
            foundPlayer = true;
        }
        GetComponent<Health>().maxHealth = config.health;
    }

    public void setSpawnLocation(Vector3 position, Quaternion rotation)
    {
        spawnPosition = position;
        spawnRotation = rotation;
    }

    public void resetLocation()
    {
        gameObject.transform.position = spawnPosition;
        gameObject.transform.rotation = spawnRotation;
    }
    void Update()
    {
        /*
        if (config.CSGoPlayer)
        {
            AimbotMode();
        }
        else
        {
            */
        SeekMode();
        //}
        //print(agent.speed);
        if (player.GetComponent<PlayerLogic>().isPitched)
        {
            //agent.speed = Mathf.Max(3.5f * enemyTimeFreezedSpeed, agent.speed * (1 - slowFactor));
            agent.speed = 0.01f;
        }
        else
        {
            agent.speed = 1.5f;
            //agent.speed = Mathf.Min(3.5f, agent.speed * (1 + slowFactor));
        }
        agent.SetDestination(lastSeenPosition);

        // enemy "announces" to have found the player only at first time
        if (!firstFinding && foundPlayer)
        {
            _audioSource.PlayOneShot(foundPlayerClips[(int)Random.Range(0, foundPlayerClips.Length - 1)]);
            firstFinding = true;
        }

        // play random walking sounds every 0.5sec (audioclips are 0.5sec long)
        if (walking && lastWalked + 0.5f < Time.time)
        {
            int value = Random.Range(0, walkingClips.Length - 1);
            AudioClip clip = walkingClips[value];
            PlayClipPitched(clip, 0.8f, 1.2f);
            lastWalked = Time.time;
        }

        Quaternion lookRotation = Quaternion.LookRotation(lastSeenPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, config.turnSpeed);
    }
    public void reset()
    {
        resetLocation();
        foundPlayer = false;
        firstFinding = false;
    }
    public void PlayClipPitched(AudioClip clip, float minPitch, float maxPitch)
    {
        // little trick to make clip sound less redundant
        _audioSource.pitch = Random.Range(minPitch, maxPitch);
        // plays same clip only once, this way no overlapping
        _audioSource.PlayOneShot(clip);
        _audioSource.pitch = 1f;
    }

    /// <summary>
    /// Looks for the player with a field of view.
    /// After amount of time starts to search the player.
    /// </summary>
    void SeekMode()
    {
        Vector3 playerDirection = target.position - transform.position;
        float rotationDifference = Vector3.Angle(transform.forward, playerDirection);

        if (Mathf.Abs(rotationDifference) <= config.fieldOfView)
        {
            if (Physics.Raycast(transform.position, playerDirection, out RaycastHit hit, playerDirection.magnitude, layerMask))
            {
                if (foundPlayer = hit.transform.Equals(target))
                {
                    lastSeenPosition = hit.transform.position;
                    transform.Rotate(0, Random.Range(-config.inaccuracy, config.inaccuracy), 0);
                }
            }
        }
        else
        {
            foundPlayer = false;
        }

        if (!foundPlayer && timeToFind >= config.timeTillSeek)
        {
            Vector3 randomDirection = Random.insideUnitSphere * config.randomStepSpeed;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, config.randomStepSpeed, 1);
            lastSeenPosition = hit.position;

            timeToFind = 0;
        }
        else if (!foundPlayer)
        {
            timeToFind += Time.deltaTime;
        }

        // checking if enemy moves to play walking-sounds
        if (oldLastPosition != lastSeenPosition) walking = true;
        else walking = false;

    }

    /// <summary>
    /// Always knows where the player is.
    /// </summary>
    void AimbotMode()
    {
        lastSeenPosition = target.position;
    }

    /// <summary>
    /// If enemy gets shot and returns fire on attack then the enemy knows the
    /// last seen position of the player.
    /// </summary>
    /// <param name="from"></param>
    public void GotShot(GameObject from)
    {
        if (!config.returnsFireOnAttack) return;
        foundPlayer = true;
        lastSeenPosition = from.transform.position;
    }

}
