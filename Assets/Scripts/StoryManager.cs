using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public uint storyBeat;
    public float beatTimer;
    public bool isBeatTimed;
    // Start is called before the first frame update
    void Start()
    {
        this.storyBeat = 0;
        this.beatTimer = 5f;
        this.isBeatTimed = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Do updates based on story beat.
        if (this.storyBeat == 0) {

        }

        this.beatTimer -= Time.deltaTime;
        if (this.isBeatTimed && this.beatTimer < 0f) {
            this.NextBeat();
        }
    }

    // Trigger the story beat (can be called from outside when actions done).
    public void NextBeat() {
        this.storyBeat += 1;
        if (this.storyBeat == 1) {
            // Prepare for story beat 1.
        }
    }
}
