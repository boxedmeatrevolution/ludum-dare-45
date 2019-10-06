using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private Animator animator;
    private new Camera camera;
    private Vector2 waypoint;
    private Item targetItem = null;

    private Machine machine;
    private OrbManager orbManager;
    private bool isTargettingMachine = false;
    
    public float pickupRange = 0.5f;
    public float walkRange = 0.05f;
    public float speed = 1;
    public float height = 1.2f;
    public Item item = null;

    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponentInChildren<Animator>();
        this.camera = FindObjectOfType<Camera>();
        this.waypoint = this.transform.position;
        this.machine = GameObject.Find("Machine").GetComponent<Machine>();
        this.orbManager = GameObject.Find("OrbManager").GetComponent<OrbManager>();
    }

    // Update is called once per frame
    void Update() {
        // Drop items if commanded.
        if (Input.GetAxis("Fire2") > 0f) {
            if (this.item != null) {
                this.item.Drop();
                this.item = null;
            }
        }

        // Look for any items being targeted by the left click.
        if (Input.GetMouseButtonDown(0)) {
            this.targetItem = null;
            this.isTargettingMachine = false;
            Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
            foreach (RaycastHit2D hit in hits) {
                Item hitItem = hit.transform.gameObject.GetComponent<Item>();
                if (hitItem != null) {
                    this.targetItem = hitItem;
                    break;
                }
            }
        }

        // Otherwise, just set the target to the position clicked... Or, go to the machine.
        if (this.targetItem == null && Input.GetMouseButton(0)) {
            
            // Check if player should go to machine
            Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
            foreach (RaycastHit2D hit in hits)
            {
                Machine machine = hit.transform.gameObject.GetComponentInParent<Machine>();
                if (machine != null)
                {
                    this.isTargettingMachine = true;
                    break;
                }
            }
            if (this.isTargettingMachine)
            {
                // Go to machine
                this.waypoint = this.machine.GetWaypoint().position;
            }
            else
            {
                // Go to mouse
                this.waypoint = this.camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        // Walk towards the target item (or point).
        Vector2 displacement = this.waypoint - (Vector2)this.transform.position;
        if (this.targetItem != null) {
            displacement = this.targetItem.transform.position - this.transform.position;
        }
            
        if (displacement.magnitude > this.walkRange) {
            Vector2 direction = displacement.normalized;
            direction *= Time.deltaTime * this.speed;
            this.transform.position += (Vector3)direction;

            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y)) {
                if (direction.y < 0) {
                    this.animator.Play("Main_Down");
                } else {
                    this.animator.Play("Main_Up");
                }
            } else {
                if (direction.x < 0) {
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
                this.isTargettingMachine = false;
            }
        }
        // If you don't already have an item, then pick it up.
        if (this.item == null && this.targetItem != null && displacement.magnitude < this.pickupRange) {
            if (this.targetItem.Pickup()) {
                this.item = this.targetItem;
            }
            this.waypoint = this.targetItem.transform.position;
            this.targetItem = null;
        }
    }
}
