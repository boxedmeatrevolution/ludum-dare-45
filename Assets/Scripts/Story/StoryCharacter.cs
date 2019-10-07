using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCharacter : MonoBehaviour
{
    private float eventStart = 0f;
    private float spawnTime = 2f;
    private float despawnTime = 2f;
    public State state = State.REGULAR;

    private Transform spawn;
    private Transform despawn;

    public enum State
    {
        REGULAR,
        COMING_OUT_OF_VOID,
        RETURNING_TO_VOID
    }

    // Start is called before the first frame update
    void Start()
    {
        this.spawn = GameObject.Find("StorySpawnPoint").GetComponentInChildren<Transform>();
        this.despawn = GameObject.Find("StoryDespawnPoint").GetComponentInChildren<Transform>();

        // Teleport myself to the void and emerge.
        eventStart = Time.realtimeSinceStartup;
        this.state = State.COMING_OUT_OF_VOID;
        this.transform.localScale = new Vector3(0, 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        float eventDelta = Time.realtimeSinceStartup - eventStart;
        if (this.state == State.COMING_OUT_OF_VOID)
        {
            float percent = eventDelta / spawnTime;

            if (percent > 1)
            {
                percent = 1f;
            }
            this.transform.localScale = new Vector3(percent, percent, 1f);
            Vector3 displacement = this.spawn.transform.position - this.despawn.transform.position;
            displacement *= percent;
            this.transform.position = this.despawn.transform.position + displacement; 

            if (percent > 0.99f)
            {
                this.state = State.REGULAR;
            } 
        }

        if (this.state == State.RETURNING_TO_VOID)
        {
            float percent = 1f - eventDelta / despawnTime;

            if (percent > 1)
            {
                percent = 1f;
            }
            this.transform.localScale = new Vector3(percent, percent, 1f);
            Vector3 displacement = this.spawn.transform.position - this.despawn.transform.position;
            displacement *= percent;
            this.transform.position = this.despawn.transform.position + displacement;

            if (percent > 0.99f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void ReturnToVoid() {
        this.eventStart = Time.realtimeSinceStartup;
        this.state = State.RETURNING_TO_VOID;
    }
}
