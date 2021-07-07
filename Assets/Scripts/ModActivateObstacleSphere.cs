using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

public class ModActivateObstaclesSphere : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        print("onTriggerEnter with c: " + collider.name);
        PantoCollider pc = collider.GetComponent<PantoCollider>();
        if (pc != null && pc.enabled)
        {
            if (pc.GetContainingSpheres() == 0)
            {
                pc.CreateObstacle();
                //if (pc.IsEnabled()) pc.Enable();
                pc.Enable();
            }
            pc.IncreaseSpheres();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        TriggerExit(collider);
    }

    public void TriggerExit(Collider collider)
    {
        print("onTriggerExit with c: " + collider.name);
        PantoCollider pc = collider.GetComponent<PantoCollider>();
        if (pc != null)
        {
            pc.DecreaseSpheres();
            if (pc.GetContainingSpheres() == 0)
            {
                pc.Remove();
            }
        }
    }
}
