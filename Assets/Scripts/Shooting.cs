using UnityEngine;
using DualPantoFramework;
using System.Collections.Generic;

public class Shooting : MonoBehaviour
{
    /*
     * TODO: 6. A clever way of keeping track of hits might be to make the 
     * damage/second dependent on how precisely you hit the opponent, rather 
     * than having a step function hit/no hit.
    */
    public bool isUpper = true;

    public float fireSpreadAngle = 2f;
    public Transform enemyTransform;

    AudioSource audioSource;
    AudioClip _currentClip;
    LineRenderer lineRenderer;
    PantoHandle handle;

    System.DateTime lastShot;
    public double reloadingTimeMillis;
    public GameObject shotPrefab;
    Rigidbody shots;

    float shotSpeed = 0.3f;
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
        shots = null;

        lineRenderer = GetComponent<LineRenderer>();

        audioSource = GetComponent<AudioSource>();

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
                // kein neuer schuss wenn noch einer existiert
                lastShot = System.DateTime.Now;
                if (shots == null && gameObject.GetComponent<EnemyLogic>().foundPlayer) //es existiert kein shot mehr
                {
                    shoot();
                }
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
        GameObject projectile = Instantiate(shotPrefab, transform.position + transform.forward / 10, transform.rotation);
        Rigidbody rigidshot = projectile.GetComponent<Rigidbody>();
        shots = rigidshot; //uerberfluessig?
        rigidshot.constraints = RigidbodyConstraints.FreezePositionY;
        rigidshot.velocity = transform.forward * shotSpeed;
        //Debug.Log(gameObject.name + "direction: " + transform.forward);
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
}
