using UnityEngine;
using DualPantoFramework;
using System.Collections.Generic;

public class Shooting : MonoBehaviour
{
    public float maxRayDistance = 20f;
    public LayerMask hitLayers;
    /*
     * TODO: 6. A clever way of keeping track of hits might be to make the 
     * damage/second dependent on how precisely you hit the opponent, rather 
     * than having a step function hit/no hit.
    */
    public int damage = 2;
    public bool isUpper = true;
    public AudioClip defaultClip;
    public AudioClip wallClip;
    public AudioClip hitClip;

    public float fireSpreadAngle = 2f;
    public Transform enemyTransform;

    AudioSource audioSource;
    AudioClip _currentClip;
    LineRenderer lineRenderer;
    PantoHandle handle;

    System.DateTime lastShot;
    public double reloadingTimeMillis;
    public GameObject shotPrefab;
    List<Rigidbody> shots;
    public int shotSpeed = 40;

    AudioClip currentClip
    {
        get => _currentClip;
        set
        {
            if (_currentClip == null) _currentClip = value;
            else if (!currentClip.Equals(value))
            {
                _currentClip = value;
                audioSource.Stop();
                audioSource.clip = value;
                audioSource.Play();
            }
        }
    }

    void Start()
    {
        lastShot = System.DateTime.Now;
        shots = new List<Rigidbody>();

        lineRenderer = GetComponent<LineRenderer>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = defaultClip;

        GameObject panto = GameObject.Find("Panto");
        if (isUpper)
        {
            handle = panto.GetComponent<UpperHandle>();
        }
        else
        {
            handle = panto.GetComponent<LowerHandle>();
        }

    }

    void Update()
    {
        if ((System.DateTime.Now - lastShot).TotalMilliseconds > reloadingTimeMillis)
        {
            if (Input.GetKeyDown(KeyCode.Space) &&
            gameObject.name == "Player")
            {
                lastShot = System.DateTime.Now;
                shoot();
            }
            else if (gameObject.name.Contains("EnemyPrefab"))
            {
                lastShot = System.DateTime.Now;
                shoot();
            }
        }

        //Fire();
        //FireCone();

    }

    /// <summary>
    /// Fire gun with aiming assistance.
    /// </summary>

    void shoot()
    {
        GameObject projectile = Instantiate(shotPrefab, transform.position + transform.forward/10, transform.rotation);
        Rigidbody rigidshot = projectile.GetComponent<Rigidbody>();
        shots.Add(rigidshot); //uerberfluessig?
        rigidshot.constraints = RigidbodyConstraints.FreezePositionY;
        rigidshot.velocity = transform.forward * shotSpeed;
        Debug.Log(gameObject.name + "direction: " + transform.forward);
        projectile.GetComponent<shotController>().shotBy = gameObject;
        if (name.Equals("Player"))
        {
            GameObject aimTo = null;
            float minRotationDifference = fireSpreadAngle;

            foreach (var gObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                if (gObj.name.Contains("EnemyPrefab")) //TODO: check name
                {
                    Vector3 enemyDirection = gObj.transform.position - transform.position;
                    float rotationDifference = Vector3.Angle(transform.forward, enemyDirection);
                    if (Mathf.Abs(rotationDifference) <= minRotationDifference)
                    {
                        aimTo = gObj;
                        minRotationDifference = Mathf.Abs(rotationDifference);
                    }
                }
            }
            if (aimTo != null)
            {
                projectile.GetComponent<shotController>().aimTo(aimTo);
            }
        }
    }

    void FireCone()
    {
        RaycastHit hit;

        // Getting upper rotation only for player interesting
        if (isUpper)
            transform.rotation = Quaternion.Euler(0, handle.GetRotation(), 0);

        Vector3 enemyDirection = enemyTransform.position - transform.position;
        float rotationDifference = Vector3.Angle(transform.forward, enemyDirection);

        if (Mathf.Abs(rotationDifference) <= fireSpreadAngle)
        {
            if (Physics.Raycast(transform.position, enemyDirection, out hit, maxRayDistance, hitLayers))
            {
                lineRenderer.SetPositions(new Vector3[] { transform.position, hit.point });

                Health enemy = hit.transform.GetComponent<Health>();

                if (enemy)
                {
                    enemy.TakeDamage(damage, gameObject);

                    currentClip = hitClip;
                }
                else
                {
                    currentClip = wallClip;
                }
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxRayDistance, hitLayers))
            {
                lineRenderer.SetPositions(new Vector3[] { transform.position,
                    hit.point });

                Health enemy = hit.transform.GetComponent<Health>();

                if (enemy)
                {
                    enemy.TakeDamage(damage, gameObject);

                    currentClip = hitClip;
                }
                else
                {
                    currentClip = wallClip;
                }
            }
            else
            {
                lineRenderer.SetPositions(new Vector3[] { transform.position,
                    transform.position + transform.forward * maxRayDistance });
                currentClip = defaultClip;
            }

        }
    }

    /// <summary>
    /// Simple firing in forward direction. Doesn't require a target.
    /// </summary>
    void Fire()
    {
        RaycastHit hit;

        if (isUpper)
            transform.rotation = Quaternion.Euler(0, handle.GetRotation(), 0);

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxRayDistance, hitLayers))
        {
            lineRenderer.SetPositions(new Vector3[] { transform.position, hit.point });

            Health enemy = hit.transform.GetComponent<Health>();

            if (enemy)
            {
                enemy.TakeDamage(damage, gameObject);

                currentClip = hitClip;
            }
            else
            {
                currentClip = wallClip;
            }
        }
        else
        {
            lineRenderer.SetPositions(new Vector3[] { transform.position,
                transform.position + transform.forward * maxRayDistance });
            currentClip = defaultClip;
        }
    }
}
