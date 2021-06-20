using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotController : MonoBehaviour
{
    public static int damage = 100;
    public GameObject shotBy;
    public static float aimBotForce = 0.01f;
    bool aiming = false;
    GameObject aimingAt;
    System.DateTime spawnTime;
    double MaxLifeTimeMillis = 20000;

    

    bool isSlowed = true;
    float slowFactor = 10;

    public AudioClip startShot;
    public AudioClip wallShot;
    public AudioClip playerShot;
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
            GetComponent<Rigidbody>().AddForce(enemyDirection * aimBotForce, ForceMode.Impulse);
            GetComponent<Rigidbody>().velocity /= (1 + aimBotForce);
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
            Destroy(gameObject, .0f);
            if (hitObject.name.Contains("EnemyPrefab"))
            {
                hitObject.transform.GetComponent<Health>().TakeDamage(damage, shotBy);
            }
            if (hitObject.name == "Player")
            {
                hitObject.transform.GetComponent<Health>().TakeDamage(damage, shotBy);
                AudioSource.PlayClipAtPoint(playerShot, this.gameObject.transform.position);

            }
            //if (hitObject.name.Contains("Obstacle"))
            //{
            //    AudioSource auS = new AudioSource();
            //    auS.maxDistance = 5;
            //    auS.rolloffMode = AudioRolloffMode.Logarithmic;
            //    auS.transform.position = this.gameObject.transform.position;
            //    auS.PlayOneShot(wallShot);
            //}



        }

    }

    private void OnDestroy()
    {
        
    }
}