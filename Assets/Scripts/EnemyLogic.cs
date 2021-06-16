using UnityEngine;
using UnityEngine.AI;

public class EnemyLogic : MonoBehaviour
{
    public Transform target;
    public LayerMask layerMask;
    public float aimbotDistance = 10f;
    public float seekingDistance = 1f;

    public float enemyTimeFreezedSpeed = 0.2f;
    public float slowFactor = 0.05f;
    public EnemyConfig config;

    bool foundPlayer = false;
    float timeToFind;

    private Vector3 spawnPosition;
    private GameObject player;
    private Quaternion spawnRotation;
    Vector3 lastSeenPosition;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = config.CSGoPlayer ? aimbotDistance : seekingDistance;
        player = GameObject.Find("Player");
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
            agent.speed = Mathf.Max(3.5f * enemyTimeFreezedSpeed, agent.speed * (1 - slowFactor));
        }
        else
        {
            agent.speed = Mathf.Min(3.5f, agent.speed * (1 + slowFactor));
        }
        agent.SetDestination(lastSeenPosition);

        Quaternion lookRotation = Quaternion.LookRotation(lastSeenPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, config.turnSpeed);
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
