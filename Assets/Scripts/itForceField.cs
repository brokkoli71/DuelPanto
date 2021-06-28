using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itForceField : DualPantoFramework.ForceField
    
{
    GameObject lower;
    protected override Vector3 GetCurrentForce(Collider other)
    {
        //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.Normalize(gameObject.transform.position - lower.transform.position));
                

        return Vector3.Normalize(gameObject.transform.position - lower.transform.position);

    }

    protected override float GetCurrentStrength(Collider other)
    {
        float distance = Vector3.Distance(lower.transform.position, gameObject.transform.position);
        float strength = Mathf.Max(0, distance - 2);
        return strength;

    }

    // Start is called before the first frame update
    void Start()
    {
        lower = GameObject.Find("ItHandle");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(gameObject.transform.position + Vector3.up, lower.transform.position + Vector3.up, Color.red, 2.5f);
    }
}
