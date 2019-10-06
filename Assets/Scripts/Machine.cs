using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    private List<Orb> orbs = new List<Orb>();
    private OrbManager orbManager;
    private Transform emptyWaypoint;
    private Transform leverWaypoint;
    private Transform slot1Waypoint;
    private Transform slot2Waypoint;
    private Transform slot1OrbWaypoint;
    private Transform slot2OrbWaypoint;
    private Transform orbPathWaypoint1;
    private Transform orbPathWaypoint2;
    private List<Transform> orbPath;

    private SpriteRenderer drawerSprite;
    private SpriteRenderer openSprite;
    private SpriteRenderer closedSprite;

    public State state;
    private float closedStartTime = 0f;

    public enum State
    {
        OPEN,
        CLOSING,
        CLOSED
    }

    // Start is called before the first frame update
    void Start()
    {
        this.orbManager = GameObject.Find("OrbManager").GetComponent<OrbManager>();
        this.emptyWaypoint = GameObject.Find("EmptyWaypoint").GetComponentInChildren<Transform>();
        this.leverWaypoint = GameObject.Find("LeverWaypoint").GetComponentInChildren<Transform>();
        this.slot1Waypoint = GameObject.Find("Slot1Waypoint").GetComponentInChildren<Transform>();
        this.slot2Waypoint = GameObject.Find("Slot2Waypoint").GetComponentInChildren<Transform>();
        this.slot1OrbWaypoint = GameObject.Find("Slot1OrbWaypoint").GetComponentInChildren<Transform>();
        this.slot2OrbWaypoint = GameObject.Find("Slot2OrbWaypoint").GetComponentInChildren<Transform>();
        this.orbPathWaypoint1 = GameObject.Find("OrbPathWaypoint1").GetComponentInChildren<Transform>();
        this.orbPathWaypoint2 = GameObject.Find("OrbPathWaypoint2").GetComponentInChildren<Transform>();

        this.orbPath = new List<Transform>();
        orbPath.Add(this.orbPathWaypoint1);
        orbPath.Add(this.orbPathWaypoint2);

        this.drawerSprite = GameObject.Find("SlotMachineDrawer").GetComponentInChildren<SpriteRenderer>();
        this.openSprite = GameObject.Find("SlotMachineOpen").GetComponentInChildren<SpriteRenderer>();
        this.closedSprite = GameObject.Find("SlotMachineClosed").GetComponentInChildren<SpriteRenderer>();


        this.state = State.OPEN;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.state == State.CLOSING)
        {
            this.closedStartTime = Time.realtimeSinceStartup;
            this.state = State.CLOSED;
        }
        else if (this.state == State.CLOSED)
        {
            this.openSprite.enabled = false;
            this.drawerSprite.enabled = false;

            if (this.orbs[0].item.state == Item.State.TRANSFORMED && this.orbs[1].item.state == Item.State.TRANSFORMED)
            {
                this.SpawnMonster();
                this.state = State.OPEN;
            }
        }
        else if (this.state == State.OPEN)
        {
            this.openSprite.enabled = true;
            this.drawerSprite.enabled = true;
        }
    }

    public Transform GetWaypoint()
    {
        if (this.orbManager.IsOrbPickedUp() && this.orbs.Count == 0)
        {
            return this.slot1Waypoint;
        }
        else if (this.orbManager.IsOrbPickedUp() && this.orbs.Count == 1)
        {
            return this.slot2Waypoint;
        } 
        else if (this.orbs.Count == 2)
        {
            return this.leverWaypoint;
        }
        return this.emptyWaypoint;
    }

    public void Interact()
    {
        if (this.state != State.OPEN)
        {
            return;
        }

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
    private void PutOrb(Orb orb)
    {
        if (this.orbs.Count == 0)
        {
            this.orbs.Add(orb);
            orb.item.PutInMachine(this.slot1OrbWaypoint);
        }
        else if (this.orbs.Count == 1)
        {
            this.orbs.Add(orb);
            orb.item.PutInMachine(this.slot2OrbWaypoint);
        }
    }

    private void PullLever()
    {
        if (this.orbs.Count == 2)
        {
            this.state = State.CLOSING;
            this.orbs[0].item.MachineLeverPull(orbPath);
            this.orbs[1].item.MachineLeverPull(orbPath);
        }
    }

    private void SpawnMonster()
    {
        if (this.orbs.Count == 2)
        {
            if (orbs[0].orbColor == Orb.OrbColor.BLUE && orbs[1].orbColor == Orb.OrbColor.BLUE)
            {
                //Ghost slug
                Debug.Log("MACHINE MAKES Ghost slug");
            }
            else if (orbs[0].orbColor == Orb.OrbColor.RED && orbs[1].orbColor == Orb.OrbColor.RED)
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

            // orbs are no longer in machine
            this.orbs.Clear();
        }
    }
}
