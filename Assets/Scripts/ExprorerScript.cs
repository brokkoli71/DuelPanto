using UnityEngine;
using DualPantoFramework;
using System;

public class ExprorerScript : MonoBehaviour
{
    PantoHandle lowerHandle, upperHandle;
    GameObject player;
    bool exploring;
    bool isPiched;
    bool isEnabled;
    public float activateDistance = 1;
    void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        player = GameObject.Find("Player");
        isEnabled = false;
        exploring = false;
    }

    void Update()
    {
        bool isHome = Vector3.Distance(upperHandle.transform.position, lowerHandle.transform.position ) < activateDistance;

        if (!isHome && !exploring) { // geht weg
            exploring = true;
            startingExploring();
        }
        if(isHome && exploring)//wieder zurueck
        {
            exploring = false;
            endExproring();
        }
        try 
        {
            isPiched = GameObject.Find("Player").GetComponent<PlayerLogic>().isPitched;
            if (isEnabled && !isPiched && isHome)//me faengt an sich zu bewegen
            {
                endExproring();
            }
            else if (!isEnabled && isPiched) //me hoert auf sich zu bewegen
            {
                enableExproring();
            }
        }
        catch(NullReferenceException)
        {
            isPiched = true;
        }
        

    }

    void enableExproring()
    {
        lowerHandle.Free();
    }
    void startingExploring()
    {
        upperHandle.Freeze();
    }
    async void endExproring()
    {
        await lowerHandle.SwitchTo(player);
    }
}
