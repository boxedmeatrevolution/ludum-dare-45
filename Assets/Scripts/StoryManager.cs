using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public Beat storyBeat;
    public uint beatIdx = 0;
    public float beatTimer;
    public bool isBeatTimed;

    public State state;
    private DialogueManager dm;


    public enum State
    {
        BEAT_FIRST_UPDATE,
        BEAT_ACTIVE,
        PROMPT,
        BEAT_COMPLETE,
    }
    public enum Beat
    {
        A, B, C, D, E, F, G
    }

    public Beat[] order = { Beat.A, Beat.B, Beat.C, Beat.D, Beat.F, Beat.G };

    // Start is called before the first frame update
    void Start()
    {
        this.storyBeat = this.order[this.beatIdx];
        this.beatTimer = 5f;
        this.isBeatTimed = false;
        this.state = State.BEAT_FIRST_UPDATE;

        this.dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        this.storyBeat = this.order[this.beatIdx];

        // Do updates based on story beat.
        if (this.storyBeat == Beat.A) {
            if (this.state == State.BEAT_FIRST_UPDATE)
            {
                dm.SetFile("intro");
                dm.StartScene("scene1");
            }
            if (this.state == State.PROMPT)
            {
                dm.StartScene("pickupPrompt");
            }
            this.state = State.BEAT_ACTIVE;
        }

        if (this.storyBeat == Beat.B)
        {
            if (this.state == State.BEAT_FIRST_UPDATE)
            {
                dm.SetFile("intro");
                dm.StartScene("s2");
            }
            if (this.state == State.PROMPT)
            {
                dm.StartScene("putdownPrompt");
            }
            this.state = State.BEAT_ACTIVE;
        }

/*        this.beatTimer -= Time.deltaTime;
        if (this.isBeatTimed && this.beatTimer < 0f) {
            this.NextBeat();
        }*/

        if (this.state == State.BEAT_FIRST_UPDATE)
        {
            this.state = State.BEAT_ACTIVE;
        }
    }

    // Trigger the story beat (can be called from outside when actions done).
    public void NextBeat() {
        this.beatIdx += 1;
        if (this.beatIdx < this.order.Length)
        {
            this.storyBeat = this.order[this.beatIdx];
            this.state = State.BEAT_FIRST_UPDATE;
        }
    }

    public void Prompt()
    {
        this.state = State.PROMPT;
    }
}
