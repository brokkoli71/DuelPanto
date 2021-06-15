using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotController : MonoBehaviour
{
    public GameObject shotBy;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //todo: zerstoeren falls zu alt
        if()
    }
    void OnCollisionEnter(Collision collision)
    {
        if(shotBy != collision.collider.gameObject)
        {
            Debug.Log("hit " + collision.collider.gameObject.name);
            Destroy(gameObject, .1f);
        }
        
    }
}