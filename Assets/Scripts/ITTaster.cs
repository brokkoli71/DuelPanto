using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITTaster : DualPantoFramework.ForceField
{
    [Range(0, 5)]
    public float strength = 2;
    float radius = 3f;
    public ITTaster()
    {
        onUpper = false;
        onLower = true;
        Debug.Log("haaaasdasdasdaa");

    }
    protected override Vector3 GetCurrentForce(Collider other)
    {
        Debug.Log("haaaaa");
        return (gameObject.transform.position - other.transform.position).normalized;

    }

    protected override float GetCurrentStrength(Collider other)
    {
        float dist = (Vector3.Distance(gameObject.transform.position, other.transform.position));
        if (dist > radius)
        {
            return Mathf.Pow(dist - radius, 2) * strength;
        }
        return 0;
    }

}
