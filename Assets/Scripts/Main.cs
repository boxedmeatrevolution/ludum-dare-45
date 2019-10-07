using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private Animator animator;
    private new Camera camera;
    private Vector2 waypoint;
    private Item targetItem = null;
    private Orb targetOrb = null;  // STORY PURPOSES ONLY
    private Monster targetMonster = null; // STORY PURPOSES ONLY
    private Gate gate;

    private Machine machine;
    private Void voido;
    private OrbManager orbManager;
    private bool isTargettingMachine = false;
    private bool isTargettingVoid = false;
    
    public float pickupRange = 0.5f;
    public float walkRange = 0.05f;
    public float speed = 1;
    public float height = 1.2f;
    public Item item = null;

    private StoryManager storyManager;
    private AudioSource audioSource;
    public AudioClip footstepSound;
    private float footstepDistance = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        this.animator = GetComponentInChildren<Animator>();
        this.camera = FindObjectOfType<Camera>();
        this.waypoint = this.transform.position;
        this.machine = GameObject.Find("Machine").GetComponent<Machine>();
        this.voido = GameObject.Find("Void").GetComponent<Void>();
        this.orbManager = GameObject.Find("OrbManager").GetComponent<OrbManager>();
        this.storyManager = GameObject.Find("StoryManager").GetComponent<StoryManager>();
        this.gate = FindObjectOfType<Gate>();
    }

    // Update is called once per frame
    void Update() {
        if (Time.timeScale < 0.0001f)
        {
            return;
        }


        if (this.storyManager.storyBeat != StoryManager.Beat.C2 
            && this.storyManager.storyBeat != StoryManager.Beat.C4    
            && this.storyManager.storyBeat != StoryManager.Beat.D2    
        ){
            // Drop items if commanded.
            if (Input.GetAxis("Fire2") > 0f)
            {
                this.DropItem();
            }

            // Look for any items being targeted by the left click.
            if (Input.GetMouseButtonDown(0))
            {
                this.targetItem = null;
                this.isTargettingMachine = false;
                this.isTargettingVoid = false;
                Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
                foreach (RaycastHit2D hit in hits)
                {
                    Item hitItem = hit.transform.gameObject.GetComponent<Item>();
                    if (hitItem != null)
                    {
                        this.targetItem = hitItem;
                        this.targetOrb = hit.transform.gameObject.GetComponent<Orb>();
                        this.targetMonster = hit.transform.gameObject.GetComponent<Monster>();
                        break;
                    }
                }
            }
        }


        // Otherwise, just set the target to the position clicked... Or, go to the machine. Or void
        if (Input.GetMouseButton(0)) {
            // Check if player should go to machine OR Void
            this.isTargettingVoid = false;
            this.isTargettingMachine = false;
            Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
            foreach (RaycastHit2D hit in hits)
            {
                Machine machine = hit.transform.gameObject.GetComponentInParent<Machine>();
                if (machine != null)
                {
                    this.isTargettingMachine = true;
                    this.targetItem = null;
                    break;
                }

                Void voido = hit.transform.gameObject.GetComponentInParent<Void>();
                if (voido != null)
                {
                    this.isTargettingVoid = true;
                    this.targetItem = null;
                    break;
                }
            }
            if (this.isTargettingMachine)
            {
                // Go to machine
                this.waypoint = this.machine.GetWaypoint();
            }
            else if (this.isTargettingVoid)
            {
                // Go to void
                this.waypoint = this.voido.GetFrontalWaypoint();
            }
            else if (this.targetItem == null)
            {
                // Go to mouse
                this.waypoint = this.camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        if (storyManager.storyBeat == StoryManager.Beat.A && storyManager.state == StoryManager.State.BEAT_ACTIVE)
        {
            bool cont = this.ForcePickupOrb(Orb.OrbColor.BLUE);
            if (!cont)
            {
                return;
            }
        }

        if (storyManager.storyBeat == StoryManager.Beat.B && storyManager.state == StoryManager.State.BEAT_ACTIVE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.targetItem = null;
                storyManager.Prompt();
                return;
            }
            if (this.targetOrb == null || this.targetOrb.item.state == Item.State.ON_GROUND)
            {
                this.storyManager.NextBeat();
            }
        }

        if (storyManager.storyBeat == StoryManager.Beat.C1)
        {
            bool cont = this.ForcePickupOrb(Orb.OrbColor.BROWN);
            if (!cont)
            {
                return;
            }
        }
        if (storyManager.storyBeat == StoryManager.Beat.C2)
        {
            this.ForcePutOrbInMachine(Orb.OrbColor.BROWN);
        }
        if (storyManager.storyBeat == StoryManager.Beat.C3)
        {
            bool cont = this.ForcePickupOrb(Orb.OrbColor.RED);
            if (!cont)
            {
                return;
            }
        }
        if (storyManager.storyBeat == StoryManager.Beat.C4)
        {
            this.ForcePutOrbInMachine(Orb.OrbColor.RED);
        }
        if (storyManager.storyBeat == StoryManager.Beat.C5)
        {
            this.ForceRunMachine();
        }
        if (storyManager.storyBeat == StoryManager.Beat.D1)
        {
            this.ForcePickupMonster();
        }
        if (storyManager.storyBeat == StoryManager.Beat.D2)
        {
            this.ForceThrowIntoVoid();
        }

        // Walk towards the target item (or point).
        Vector2 displacement = this.waypoint - (Vector2)this.transform.position;
        if (this.targetItem != null) {
            displacement = this.targetItem.transform.position - this.transform.position;
        }
            
        if (displacement.magnitude > this.walkRange) {
            float x1 = this.gate.transform.position.x;
            float x2 = x1 + this.gate.width;
            float y1 = this.gate.transform.position.y;
            float y2 = y1 + this.gate.height;
            float cx = 0.5f * (x1 + x2);
            Vector2 position = this.transform.position;
            Vector2 deltaPos = displacement.normalized * this.speed * Time.deltaTime;
            bool inGateY = (position.y > y1 && position.y < y2);
            if (!inGateY) {
                if (position.x + deltaPos.x > x1 && position.x < cx && deltaPos.x > 0f) {
                    deltaPos.x = x1 - position.x;
                }
                if (position.x + deltaPos.x < x2 && position.x > cx && deltaPos.x < 0f) {
                    deltaPos.x = x2 - position.x;
                }
            }
            this.transform.position += (Vector3)deltaPos;
            this.footstepDistance -= deltaPos.magnitude;
            if (this.footstepDistance < 0f) {
                this.footstepDistance = 0.65f;
                this.audioSource.PlayOneShot(this.footstepSound, 0.01f);
            }

            if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y)) {
                if (deltaPos.y < 0) {
                    this.animator.Play("Main_Down");
                } else {
                    this.animator.Play("Main_Up");
                }
            } else {
                if (deltaPos.x < 0) {
                    this.animator.Play("Main_Left");
                } else {
                    this.animator.Play("Main_Right");
                }
            }
        } else {
            this.animator.Play("Main_Idle");

            // If character has made it to the machine, then interact with the machine
            if (this.isTargettingMachine)
            {
                this.machine.Interact();
                this.item = null;
                this.isTargettingMachine = false;
            }

            // If character made it to void
            if (this.isTargettingVoid)
            {
                this.voido.Interact();
                this.item = null;
                this.isTargettingVoid = false;
            }
        }
        // If you don't already have an item, then pick it up.
        if (this.item == null && this.targetItem != null && displacement.magnitude < this.pickupRange) {
            if (this.targetItem.Pickup()) {
                this.item = this.targetItem;
                this.item.audioSource.PlayOneShot(this.item.pickupSound, 0.5f);
            }
            this.waypoint = this.targetItem.transform.position;
            this.targetItem = null;
        }

        // Maintain z ordering.
        Vector2 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, pos.y, pos.y / 300f);
    }

    public void DropItem()
    {
        if (this.item != null)
        {
            this.item.Drop();
            this.item = null;
        }
    }

    private bool ForcePickupOrb(Orb.OrbColor color)
    {
        if ((this.targetOrb == null || this.targetOrb.orbColor != color) && Input.GetMouseButtonDown(0))
        {
            this.targetItem = null;
            storyManager.Prompt();
            return false;
        }
        if (this.orbManager.IsOrbPickedUp())
        {
            Orb orb = this.orbManager.GetPickedUpOrb();
            if (orb.orbColor == color && orb.item.state == Item.State.PICKED_UP)
            {
                this.storyManager.NextBeat();
            }
        }
        return true;
    }
    private bool ForcePutOrbInMachine(Orb.OrbColor color)
    {
        if (this.targetOrb != null && this.targetOrb.orbColor == color && this.targetOrb.item.state == Item.State.PICKED_UP && Input.GetMouseButtonDown(0) && !this.isTargettingMachine)
        {
            this.targetItem = null;
            storyManager.Prompt();
            return false;
        }
        if (this.targetOrb.item.state == Item.State.IN_MACHINE)
        {
            this.storyManager.NextBeat();
        }
        return true;
    }

    private bool ForceRunMachine()
    {
        if (!this.isTargettingMachine && Input.GetMouseButtonDown(0))
        {
            this.targetItem = null;
            storyManager.Prompt();
            return false;
        }
        if (FindObjectsOfType<Monster>().Length > 0)
        {
            this.storyManager.NextBeat();
        }
        return true;
    }

    private bool ForcePickupMonster()
    {
        if (this.targetMonster == null && Input.GetMouseButtonDown(0))
        {
            this.targetItem = null;
            storyManager.Prompt();
            return false;
        }

        Monster pickedUp = null;
        foreach (Monster monster in FindObjectsOfType<Monster>())
        {
            if (monster != null && monster.GetItem() != null && monster.GetItem().state == Item.State.PICKED_UP)
            {
                pickedUp = monster;
                break;
            }
        }

        if (pickedUp != null)
        {
            this.storyManager.NextBeat();
        }
        return true;
    }

    private bool ForceThrowIntoVoid()
    {
        if (!this.isTargettingVoid && Input.GetMouseButtonDown(0))
        {
            this.targetItem = null;
            storyManager.Prompt();
            return false;
        }

        int count = 0;
        Orb[] orbs = this.orbManager.orbs;
        for (int i = 0; i < orbs.Length; i++)
        {
            Orb orb = orbs[i];
            if (orb.orbColor != Orb.OrbColor.WHITE && orb.item.state == Item.State.ON_GROUND)
            {
                count++;
            }
        }
        if (count == 9)
        {
            this.storyManager.NextBeat();
        }
        return true;
    }
}
