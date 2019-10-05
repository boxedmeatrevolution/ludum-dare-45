using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private new Camera camera;
    private Vector2 waypoint;
    private Item targetItem = null;
    public float pickupRange = 0.5f;
    public float walkRange = 0.05f;
    public float speed = 1;
    public float height = 1.2f;
    public Item item = null;

    // Start is called before the first frame update
    void Start()
    {
        this.camera = FindObjectOfType<Camera>();
        this.waypoint = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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

        // Otherwise, just set the target to the position clicked.
        if (this.targetItem == null && Input.GetMouseButton(0)) {
            Vector3 mousePosition = this.camera.ScreenToWorldPoint(Input.mousePosition);
            this.waypoint = mousePosition;
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
        }
        // If you don't already have an item, then pick it up.
        if (this.item == null && this.targetItem != null && displacement.magnitude < this.pickupRange) {
            this.item = this.targetItem;
            this.item.Pickup();
            this.waypoint = this.targetItem.transform.position;
            this.targetItem = null;
        }
    }
}
