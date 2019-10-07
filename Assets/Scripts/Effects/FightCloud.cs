using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightCloud : MonoBehaviour
{
    public Monster fighter1 = null;
    public Monster fighter2 = null;
    public float swapTimer = 0f;
    public float rotationSpeed = 90f;
    // Start is called before the first frame update
    void Start()
    {
        this.swapTimer = 0.4f;
        this.rotationSpeed = Random.Range(-180f, 180f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 averagePosition = 0.5f * (this.fighter1.transform.position + this.fighter2.transform.position);
        this.transform.position = averagePosition;
        this.transform.rotation *= Quaternion.Euler(0f, 0f, this.rotationSpeed * Time.deltaTime);
        // Choose z position.
        Vector2 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, pos.y, -10f);
        this.swapTimer -= Time.deltaTime;
        if (this.swapTimer < 0f) {
            this.swapTimer = 0.4f;
            this.transform.localScale = new Vector3(
                2f * Mathf.Round(Random.value) - 1f,
                2f * Mathf.Round(Random.value) - 1f, 1f);
            this.rotationSpeed = Random.Range(-180f, 180f);
        }
    }
}
