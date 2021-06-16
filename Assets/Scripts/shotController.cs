using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotController : MonoBehaviour
{
    public static int damage = 100;
    public GameObject shotBy;
    public static float aimBotForce = 0.1f;
    bool aiming = false;
    GameObject aimingAt;
    System.DateTime spawnTime;
    double MaxLifeTimeMillis = 20000;

    

    bool isSlowed = false;
    float slowFactor = 4;

    public AudioClip startShot;
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = System.DateTime.Now;
        gameObject.GetComponent<AudioSource>().PlayOneShot(startShot);
    }

    // Update is called once per frame
    void Update()
    {
        if ((!GameObject.Find("Panto").GetComponent<GameManager>().gameRunning) || 
            MaxLifeTimeMillis <= (System.DateTime.Now - spawnTime).TotalMilliseconds)
        {
            Destroy(gameObject);
            return;
        }
        if (GameObject.Find("Player").GetComponent<PlayerLogic>().isPitched)
        {
            if (!isSlowed)
            {
                gameObject.GetComponent<Rigidbody>().velocity /= slowFactor;
                isSlowed = true;
            }
        }
        else
        {
            if (isSlowed)
            {
                gameObject.GetComponent<Rigidbody>().velocity *= slowFactor;
                isSlowed = false;
            }
        }

    }

    private void FixedUpdate()
    {
        if (aiming)
        {
            Vector3 enemyDirection = aimingAt.transform.position - transform.position;
            GetComponent<Rigidbody>().AddForce(enemyDirection * aimBotForce);
        }

    }

    public void aimTo(GameObject aimingAt)
    {
        this.aimingAt = aimingAt;
        aiming = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.collider.gameObject;
        if (shotBy.name != hitObject.name)
        {
            Debug.Log("hit " + hitObject.name);
            Destroy(gameObject, .01f);
            if (hitObject.name.Contains("EnemyPrefab") || hitObject.name == "Player")
            { hitObject.transform.GetComponent<Health>().TakeDamage(damage, shotBy); }


        }

    }
}