using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    private List<Orb> orbs = new List<Orb>();
    private OrbManager orbManager;
    private Transform emptyWaypoint;
    private Transform slot1Waypoint;
    private Transform slot2Waypoint;

    // Start is called before the first frame update
    void Start()
    {
        this.orbManager = GameObject.Find("OrbManager").GetComponent<OrbManager>();
        this.emptyWaypoint = GameObject.Find("EmptyWaypoint").GetComponentInChildren<Transform>();
        this.slot1Waypoint = GameObject.Find("Slot1Waypoint").GetComponentInChildren<Transform>();
        this.slot2Waypoint = GameObject.Find("Slot2Waypoint").GetComponentInChildren<Transform>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform GetWaypoint()
    {
        if (!this.orbManager.IsOrbPickedUp())
        {
            return this.emptyWaypoint;
        }
        else if (this.orbs.Count == 0)
        {
            return this.slot1Waypoint;
        }
        else if (this.orbs.Count == 1)
        {
            return this.slot2Waypoint;
        } 
        else
        {
            return this.emptyWaypoint;
        }
    }

    public void Interact()
    {
        if (this.orbManager.IsOrbPickedUp() && this.orbs.Count < 2)
        {
            Debug.Log("Loading orb");
            this.PutOrb(this.orbManager.GetPickedUpOrb());
        }
        else if (this.orbs.Count == 2)
        {
            Debug.Log("Pulling lever");
            this.PullLever();
        }
    }
    public void PutOrb(Orb orb)
    {
        if (this.orbs.Count <= 2)
        {
            this.orbs.Add(orb);
        }
    }

    public void PullLever()
    {
        if (this.orbs.Count == 2)
        {
            if (orbs[0].orbColor == Orb.OrbColor.BLUE && orbs[1].orbColor == Orb.OrbColor.BLUE)
            {
                //Ghost slug
                Debug.Log("MACHINE MAKES Ghost slug");
            }
            else if(orbs[0].orbColor == Orb.OrbColor.RED && orbs[1].orbColor == Orb.OrbColor.RED)
            {
                //Living flame
                Debug.Log("MACHINE MAKES Living flame");

            }
            else if (orbs[0].orbColor == Orb.OrbColor.BROWN && orbs[1].orbColor == Orb.OrbColor.BROWN)
            {
                //Clay copycat
                Debug.Log("MACHINE MAKES Clay copycat");
            }
            else if ((orbs[0].orbColor == Orb.OrbColor.BLUE && orbs[1].orbColor == Orb.OrbColor.RED) || (orbs[0].orbColor == Orb.OrbColor.RED && orbs[1].orbColor == Orb.OrbColor.BLUE))
            {
                //Goblin
                Debug.Log("MACHINE MAKES Goblin");
            }
            else if ((orbs[0].orbColor == Orb.OrbColor.BLUE && orbs[1].orbColor == Orb.OrbColor.BROWN) || (orbs[0].orbColor == Orb.OrbColor.BROWN && orbs[1].orbColor == Orb.OrbColor.BLUE))
            {
                //Plant Ogre
                Debug.Log("MACHINE MAKES Plant Ogre");
            }
            else if ((orbs[0].orbColor == Orb.OrbColor.RED && orbs[1].orbColor == Orb.OrbColor.BROWN) || (orbs[0].orbColor == Orb.OrbColor.BROWN && orbs[1].orbColor == Orb.OrbColor.RED))
            {
                //Fire salamander
                Debug.Log("MACHINE MAKES Fire salamander");
            }

            // orbs are now monsters
            this.orbs.Clear();
        }
    }
}
