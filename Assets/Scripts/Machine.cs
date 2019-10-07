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
    private Transform spawnWaypoint;
    private List<Vector3> orbPath;

    private SpriteRenderer drawerSprite;
    private SpriteRenderer openSprite;
    private SpriteRenderer closedSprite;

    private Void voido;

    public State state;
    private float closedStartTime = 0f;

    public AudioClip lever1Sound;
    public AudioClip lever2Sound;
    public AudioClip loadOrb1Sound;
    public AudioClip loadOrb2Sound;
    private AudioSource audioSource;

    private float voidSoundTime = 0f;

    public enum State
    {
        OPEN,
        CLOSING,
        CLOSED
    }

    // Start is called before the first frame update
    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        this.orbManager = GameObject.Find("OrbManager").GetComponent<OrbManager>();
        this.emptyWaypoint = GameObject.Find("EmptyWaypoint").GetComponentInChildren<Transform>();
        this.leverWaypoint = GameObject.Find("LeverWaypoint").GetComponentInChildren<Transform>();
        this.slot1Waypoint = GameObject.Find("Slot1Waypoint").GetComponentInChildren<Transform>();
        this.slot2Waypoint = GameObject.Find("Slot2Waypoint").GetComponentInChildren<Transform>();
        this.slot1OrbWaypoint = GameObject.Find("Slot1OrbWaypoint").GetComponentInChildren<Transform>();
        this.slot2OrbWaypoint = GameObject.Find("Slot2OrbWaypoint").GetComponentInChildren<Transform>();
        this.orbPathWaypoint1 = GameObject.Find("OrbPathWaypoint1").GetComponentInChildren<Transform>();
        this.orbPathWaypoint2 = GameObject.Find("OrbPathWaypoint2").GetComponentInChildren<Transform>();
        this.spawnWaypoint = GameObject.Find("SpawnWaypoint").GetComponentInChildren<Transform>();

        this.orbPath = new List<Vector3>();
        orbPath.Add(this.orbPathWaypoint1.position);
        orbPath.Add(this.orbPathWaypoint2.position);

        this.drawerSprite = GameObject.Find("SlotMachineDrawer").GetComponentInChildren<SpriteRenderer>();
        this.openSprite = GameObject.Find("SlotMachineOpen").GetComponentInChildren<SpriteRenderer>();
        this.closedSprite = GameObject.Find("SlotMachineClosed").GetComponentInChildren<SpriteRenderer>();

        this.voido = GameObject.Find("Void").GetComponent<Void>();

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
            this.voidSoundTime -= Time.deltaTime;
            if (this.voidSoundTime < 0f) {
                this.voido.audioSource.PlayOneShot(this.voido.voidSummonClip, 0.75f);
                this.voidSoundTime = float.PositiveInfinity;
            }
        }
        else if (this.state == State.OPEN)
        {
            this.openSprite.enabled = true;
            this.drawerSprite.enabled = true;
        }
    }

    public Vector3 GetWaypoint()
    {
        if (this.orbManager.IsOrbPickedUp() && this.orbs.Count == 0)
        {
            return this.slot1Waypoint.position;
        }
        else if (this.orbManager.IsOrbPickedUp() && this.orbs.Count == 1)
        {
            return this.slot2Waypoint.position;
        } 
        else if (this.orbs.Count == 2 && this.state == State.OPEN)
        {
            return this.leverWaypoint.position;
        }
        return this.emptyWaypoint.position;
    }

    public void Interact()
    {
        if (this.state != State.OPEN)
        {
            return;
        }

        if (this.orbManager.IsOrbPickedUp() && this.orbs.Count < 2)
        {
            this.PutOrb(this.orbManager.GetPickedUpOrb());
        }
        else if (this.orbs.Count == 2)
        {
            this.PullLever();
        }
    }
    private void PutOrb(Orb orb)
    {
        if (this.orbs.Count == 0)
        {
            this.audioSource.PlayOneShot(this.loadOrb1Sound, 0.5f);
            this.orbs.Add(orb);
            orb.item.PutInMachine(this.slot1OrbWaypoint.position);
        }
        else if (this.orbs.Count == 1)
        {
            this.audioSource.PlayOneShot(this.loadOrb2Sound, 0.5f);
            this.orbs.Add(orb);
            orb.item.PutInMachine(this.slot2OrbWaypoint.position);
        }
    }

    bool leverCount = false;
    private void PullLever()
    {
        if (this.orbs.Count == 2)
        {
            this.voidSoundTime = 1f;
            this.leverCount = !this.leverCount;
            this.audioSource.PlayOneShot(leverCount ? this.lever1Sound : this.lever2Sound, 0.5f);
            this.state = State.CLOSING;
            this.orbs[0].item.MachineLeverPull(orbPath);
            this.orbs[1].item.MachineLeverPull(orbPath);
        }
    }

    private void SpawnMonster()
    {
        if (this.orbs.Count == 2) {
            GameObject spawnedObj = null;
            if (orbs[0].orbColor == Orb.OrbColor.BLUE && orbs[1].orbColor == Orb.OrbColor.BLUE)
            {
                spawnedObj = Instantiate(PrefabManager.GHOST_SLUG_PREFAB, (Vector2)this.spawnWaypoint.transform.position, Quaternion.identity);
            }
            else if (orbs[0].orbColor == Orb.OrbColor.RED && orbs[1].orbColor == Orb.OrbColor.RED) {
                spawnedObj = Instantiate(PrefabManager.LIVING_FLAME_PREFAB, (Vector2)this.spawnWaypoint.transform.position, Quaternion.identity);
            }
            else if (orbs[0].orbColor == Orb.OrbColor.BROWN && orbs[1].orbColor == Orb.OrbColor.BROWN) {
                spawnedObj = Instantiate(PrefabManager.COPYCAT_PREFAB, (Vector2)this.spawnWaypoint.transform.position, Quaternion.identity);
            }
            else if ((orbs[0].orbColor == Orb.OrbColor.BLUE && orbs[1].orbColor == Orb.OrbColor.RED) || (orbs[0].orbColor == Orb.OrbColor.RED && orbs[1].orbColor == Orb.OrbColor.BLUE)) {
                spawnedObj = Instantiate(PrefabManager.GOBLIN_PREFAB, (Vector2)this.spawnWaypoint.transform.position, Quaternion.identity);
            }
            else if ((orbs[0].orbColor == Orb.OrbColor.BLUE && orbs[1].orbColor == Orb.OrbColor.BROWN) || (orbs[0].orbColor == Orb.OrbColor.BROWN && orbs[1].orbColor == Orb.OrbColor.BLUE)) {
                spawnedObj = Instantiate(PrefabManager.PLANT_OGRE_PREFAB, (Vector2)this.spawnWaypoint.transform.position, Quaternion.identity);
            }
            else if ((orbs[0].orbColor == Orb.OrbColor.RED && orbs[1].orbColor == Orb.OrbColor.BROWN) || (orbs[0].orbColor == Orb.OrbColor.BROWN && orbs[1].orbColor == Orb.OrbColor.RED)) {
                spawnedObj = Instantiate(PrefabManager.FIRE_SALAMANDER_PREFAB, (Vector2)this.spawnWaypoint.transform.position, Quaternion.identity);
            }
            else if ((orbs[0].orbColor == Orb.OrbColor.WHITE && orbs[1].orbColor != Orb.OrbColor.WHITE) || (orbs[0].orbColor != Orb.OrbColor.WHITE && orbs[1].orbColor == Orb.OrbColor.WHITE)) {
                spawnedObj = Instantiate(PrefabManager.GHOST_PREFAB, (Vector2)this.spawnWaypoint.transform.position, Quaternion.identity);
            }

            if (spawnedObj != null)
            {
                Monster spawnedMonster = spawnedObj.GetComponent<Monster>();
                if (spawnedMonster != null)
                {
                    Orb[] monsterOrbs = { this.orbs[0], this.orbs[1] };
                    spawnedMonster.GiveOrbs(monsterOrbs);
                }

                this.voido.EjectMonster(spawnedMonster);
            }

            // orbs are no longer in machine
            this.orbs.Clear();
        }
    }
}
