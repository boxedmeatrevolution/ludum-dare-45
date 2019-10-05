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
    private State state;

    private enum State {
        ON_GROUND,
        BEING_LIFTED,
        PICKED_UP,
        BEING_DROPPED
    }

    // Start is called before the first frame update
    void Start()
    {
        this.main = FindObjectOfType<Main>();
        this.sprite = GetComponentInChildren<SpriteRenderer>();
        this.state = State.ON_GROUND;
        this.pickupVelZ = 0f;
        this.pickupZ = 0f;
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

        Vector3 newSpritePos = this.sprite.transform.localPosition;
        newSpritePos.y = this.pickupZ;
        this.sprite.transform.localPosition = newSpritePos;
    }

    public void Pickup() {
        this.pickedUp = true;
        this.state = State.BEING_LIFTED;
    }

    public void Drop() {
        this.pickedUp = false;
        this.state = State.BEING_DROPPED;
    }
}
