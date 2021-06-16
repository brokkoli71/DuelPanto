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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //todo: zerstoeren falls zu alt

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
        if (shotBy.name != collision.collider.gameObject.name)
        {
            Debug.Log("hit " + collision.collider.gameObject.name);
            Destroy(gameObject, .01f);
            if (collision.collider.gameObject.name.Contains("EnemyPrefab"))
                collision.collider.gameObject.transform.GetComponent<Health>().TakeDamage(damage, shotBy);
        }

    }
}