using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itForceField : DualPantoFramework.ForceField
    
{
    GameObject lower, upper;
    protected override Vector3 GetCurrentForce(Collider other)
    {
        //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.Normalize(gameObject.transform.position - lower.transform.position));
                Debug.Log("test");

        Debug.Log(Vector3.Normalize(lower.transform.position - upper.transform.position));
        return Vector3.Normalize(lower.transform.position - upper.transform.position);

    }

    protected override float GetCurrentStrength(Collider other)
    {
        return Vector3.Distance(gameObject.transform.position, lower.transform.position) * 10;

    }

    // Start is called before the first frame update
    void Start()
    {
        lower = GameObject.FindGameObjectWithTag("ItHandle");
        upper = GameObject.FindGameObjectWithTag("MeHandle");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(upper.transform.position + Vector3.up, lower.transform.position + Vector3.up, Color.red, float.PositiveInfinity);
    }
}
