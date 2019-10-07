using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Vector2 target;
    private float rotationSpeed;
    private Vector2 velocity;
    private float lifetime = 1.5f;
    private new SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        this.rotationSpeed = Random.Range(-180f, 180f);
        Vector2 displacement = target - (Vector2)this.transform.position;
        this.velocity = displacement * 0.5f + 1.5f * Random.insideUnitCircle;
        this.renderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        this.lifetime -= Time.deltaTime;
        this.transform.rotation *= Quaternion.Euler(0f, 0f, this.rotationSpeed * Time.deltaTime);
        Vector2 displacement = target - (Vector2)this.transform.position;
        if (displacement.magnitude < 0.5 || this.lifetime < 0f) {
            Destroy(this.gameObject);
        } else {
            this.velocity += displacement.normalized * 10f * Time.deltaTime;
        }
        this.transform.position += (Vector3)this.velocity * Time.deltaTime;
        this.renderer.color = new Color(1f, 1f, 1f, this.lifetime / 1.5f);
    }
}
