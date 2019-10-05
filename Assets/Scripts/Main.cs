using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private new Camera camera;
    private Vector2 waypoint;
    public float speed = 1;


    // Start is called before the first frame update
    void Start()
    {
        this.camera = FindObjectOfType<Camera>();
        this.waypoint = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = this.camera.ScreenToWorldPoint(Input.mousePosition);
            this.waypoint = mousePosition;
        }


        Vector2 displacement = this.waypoint - (Vector2)this.transform.position;
        if (displacement.magnitude > 0.01) {
            Vector2 direction = displacement.normalized;
            direction *= Time.deltaTime * this.speed;
            this.transform.position += (Vector3)direction;
        }
    }
}
