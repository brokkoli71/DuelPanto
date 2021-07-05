using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

public class ActivaterLogic : MonoBehaviour
{
    private PantoHandle upperHandle;

    // Start is called before the first frame update
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = upperHandle.HandlePosition(transform.position);
    }
}
