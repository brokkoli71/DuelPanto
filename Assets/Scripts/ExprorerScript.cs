using System.Threading.Tasks;
using UnityEngine;
using DualPantoFramework;
using System;

public class ExprorerScript : MonoBehaviour
{
    GameObject movingField;
    PantoCollider pantoCollider;

    PantoHandle lowerHandle, upperHandle;
    public GameObject player;
    GameObject explorer;
    bool meMoving = false;
    public float activateDistance = 1;

    const int EXPLORING = 0;
    const int HOME = 1;
    const int BOUND = 2;

    DateTime lastStateChange;

    int state = HOME;
    void Start()
    {
        /*movingField = GameObject.Find("it-movefield");
        pantoCollider = movingField.GetComponent<PantoCircularCollider>();
        pantoCollider.CreateObstacle();
        pantoCollider.Enable();*/

        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>(); //it
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>(); //me
        explorer = GameObject.Find("explorer");
        lastStateChange = System.DateTime.Now;
        /*isEnabled = true;
        exploring = false;*/
    }

    void Update()
    {
        bool isHome = Math.Abs(Vector3.Distance(player.transform.position, explorer.transform.position )) < activateDistance;
        print("asd " + Vector3.Distance(explorer.transform.position, player.transform.position) + " " + explorer.transform.position + " " + player.transform.position);
        try
        {
            meMoving = !GameObject.Find("Player").GetComponent<PlayerLogic>().isPitched;
        }
        catch (NullReferenceException)
        {
            print("asd fehler");
        }

        if (Input.GetKey(KeyCode.Y))
        {
            state = HOME;
            lowerHandle.Free();
            upperHandle.Free();
            return;
        }
        if (System.DateTime.Now.Millisecond - lastStateChange.Millisecond > 100)//nur alle 100 ms
        {
            switch (state)
            {
                case EXPLORING:
                    if (isHome)
                    {
                        print("asd HOME");
                        state = HOME;
                        upperHandle.Free();
                    }

                    break;
                case HOME:
                    if (!isHome)
                    {
                        print("asd EXPLORING");
                        state = EXPLORING;
                        upperHandle.Freeze();
                        enableColider();
                    }
                    if (meMoving)
                    {
                        print("asd BOUND");
                        state = BOUND;
                        //lowerHandle.Freeze();
                        //lowerHandle.SwitchTo(player);
                        disableColider();
                    }
                    break;
                case BOUND:
                    if (!meMoving&&isHome)
                    {
                        print("asd HOME");
                        state = HOME;
                        lowerHandle.Free();
                    }
                    break;
            }
        }
        
    }


    void enableColider()
    {
        print("asd enableColider");
    }
    void disableColider()
    {
        print("asd disableColider");

    }

}
