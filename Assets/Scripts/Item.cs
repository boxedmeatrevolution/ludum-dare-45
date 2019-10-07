using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool pickedUp = false;

    private Main main;
    private SpriteRenderer sprite;
    private float pickupAccel = 5.0f;
    private float pickupVelZ;
    private float pickupZ;
    private Vector2 initialOffset;
    public State state;

    private float initialZ;
    private Vector3 initialPosition;

    private Vector3 waypoint; // a single location to go to...
    private List<Vector3> waypointPath; //many locations to go to...
    private int pathIdx;
    private float waypointAccel = 0.0005f;
    private float waypointVel = 0.0f;

    public enum State {
        ON_GROUND,
        BEING_LIFTED,
        PICKED_UP,
        BEING_DROPPED,
        
        // Orbs only
        BEING_PUT_IN_MACHINE,
        IN_MACHINE,
        TRANSFORMING, 
        TRANSFORMED, 
        RETURNING_TO_LAB,

        // Monsters only
        VOIDING
    }

    // Start is called before the first frame update
    void Start()
    {
        this.main = FindObjectOfType<Main>();
        this.sprite = GetComponentInChildren<SpriteRenderer>();
        this.initialOffset = this.sprite.transform.localPosition;
        this.state = State.ON_GROUND;
        this.pickupVelZ = 0f;
        this.pickupZ = 0f;
        this.waypointVel = 0f;
        this.initialZ = this.transform.position.z;
        this.initialPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.state == State.BEING_LIFTED) {
            // Calculate the parabola that takes this object right into the main characters hands.
            float pickupVelZSq = 2f * this.pickupAccel * (this.main.height - this.pickupZ);
            if (pickupVelZSq > 0f) {
                this.pickupVelZ = Mathf.Sqrt(pickupVelZSq);
                float deltaT = this.pickupVelZ / this.pickupAccel;
                Vector2 displacement = this.transform.position - this.main.transform.position;
                if (Mathf.Abs(deltaT) < 0.0001f) {
                    this.state = State.PICKED_UP;
                }
                else {
                    this.pickupZ += this.pickupVelZ * Time.deltaTime;
                    this.transform.position -= (Vector3)displacement / deltaT * Time.deltaTime;
                }
            }
            else {
                this.state = State.PICKED_UP;
            }
        }
        if (this.state == State.PICKED_UP) {
            this.transform.position = this.main.transform.position;
            this.pickupZ = this.main.height;
        }
        if (this.state == State.BEING_DROPPED) {
            if (this.pickupZ > 0f) {
                this.pickupVelZ -= this.pickupAccel * Time.deltaTime;
            }
            else if (this.pickupZ < 0f && this.pickupVelZ < 0f) {
                this.pickupVelZ *= -0.5f;
            }
            this.pickupZ += this.pickupVelZ * Time.deltaTime;
            if (Mathf.Abs(this.pickupZ) < 0.01f && Mathf.Abs(this.pickupVelZ) < 1f) {
                this.state = State.ON_GROUND;
            }
        }
        if (this.state == State.ON_GROUND) {
            this.pickupZ = 0f;
        }
        if (this.state == State.BEING_PUT_IN_MACHINE)
        {
            this.SetZIndex(6); // this puts it between the drawer of the machine and the machine
            if (this.pickupZ > 0.00001f)
            {
                // undo the pickupz but keep the ball in place
                Vector3 removePickupZ = new Vector3(0, this.pickupZ, 0);
                this.transform.position += removePickupZ;
                this.pickupZ = 0f;
            }
            this.MoveToWaypoint();
            if (this.IsAtWaypoint())
            {
                this.state = State.IN_MACHINE;
            }
        }
        if (this.state == State.TRANSFORMING)
        {
            this.SetZIndex(10); // behind the machine
            this.MoveAlongPath();
            if (this.IsAtEndOfPath())
            {
                this.state = State.TRANSFORMED;
                this.SetZIndex(this.initialZ);
            }
        }
        if (this.state == State.RETURNING_TO_LAB)
        {
            this.waypoint = this.initialPosition;
            this.MoveToWaypoint();
            if (this.IsAtWaypoint())
            {
                this.state = State.ON_GROUND;
            }
        }

        Vector3 newSpritePos = this.initialOffset;
        newSpritePos.y += this.pickupZ;
        this.sprite.transform.localPosition = newSpritePos;
    }

    public bool Pickup() {
        Monster monster = this.GetComponent<Monster>();
        if (monster == null || monster.CanPickup()) {
            if (this.state != State.BEING_PUT_IN_MACHINE 
                && this.state != State.IN_MACHINE
                && this.state != State.RETURNING_TO_LAB
                && this.state != State.TRANSFORMED
                && this.state != State.TRANSFORMING
            ) {
                this.pickedUp = true;
                this.state = State.BEING_LIFTED;
                return true;
            }
        }
        return false;
    }

    public void Drop() {
        this.pickedUp = false;
        this.state = State.BEING_DROPPED;
    }

    private void MoveToWaypoint()
    {
        if (this.waypoint == null)
        {
            Debug.Log("Cannot move to waypoint: waypoint is null");
            return;
        }
        Vector2 displacement = this.waypoint - this.transform.position;
        Vector2 direction = displacement.normalized;
        this.waypointVel += this.waypointAccel;
        Vector2 delta = direction * waypointVel;
        if (delta.magnitude > displacement.magnitude) {
            // move the rest of the way
            this.transform.position += (Vector3)displacement;
        }
        else {
            // move a bit closer
            this.transform.position += (Vector3)delta;
        }

        if (this.IsAtWaypoint())
        {
            this.waypointVel = 0f;
        }
    }

    private void MoveAlongPath()
    {
        if (this.waypointPath == null)
        {
            Debug.Log("Cannot along path: path is null");
            return;
        }
        this.waypoint = this.waypointPath[this.pathIdx];
        this.MoveToWaypoint();
        if (this.IsAtWaypoint())
        {
            if (!this.IsAtEndOfPath())
            {
                this.pathIdx++;
            }
        }
    }

    private bool IsAtWaypoint()
    {
        if (this.waypoint == null)
        {
            Debug.Log("Cannot move to waypoint: waypoint is null");
            return true;
        }
        return ((Vector2)this.waypoint - (Vector2)this.transform.position).magnitude < 0.0001f;
    }

    private bool IsAtEndOfPath()
    {
        if (this.waypointPath == null)
        {
            Debug.Log("Cannot along path: path is null");
            return true;
        }

        return this.pathIdx == this.waypointPath.Count - 1 
            && ((Vector2)this.waypointPath[this.waypointPath.Count - 1] - (Vector2)this.transform.position).magnitude < 0.0001f;
    }

    public void PutInMachine(Vector3 transform)
    {
        this.pickedUp = false;
        this.state = State.BEING_PUT_IN_MACHINE;
        this.waypoint = transform;
    }

    public void MachineLeverPull(List<Vector3> path)
    {
        this.pickedUp = false;
        this.state = State.TRANSFORMING;
        this.waypointPath = path;
        this.pathIdx = 0;
    }

    void SetZIndex(float z)
    {
        Vector3 setZ = this.transform.position;
        setZ.z = z; 
        this.transform.position = setZ;
    }

    //For ORBS ONLY
    public void ReturnToLab()
    {
        this.state = State.RETURNING_TO_LAB;
        this.pickedUp = false;
    }    
    
    //For MONSTERS ONLY
    public void SendToVoid()
    {
        this.state = State.VOIDING;
        if (this.pickupZ > 0.00001f)
        {
            // undo the pickupz but keep the ball in place
            Vector3 removePickupZ = new Vector3(0, this.pickupZ, 0);
            this.transform.position += removePickupZ;
            this.pickupZ = 0f;
        }
        this.pickedUp = false;
    }
    public void SpawnFromVoid()
    {
        this.state = State.VOIDING;
        this.pickedUp = false;
    }
}
