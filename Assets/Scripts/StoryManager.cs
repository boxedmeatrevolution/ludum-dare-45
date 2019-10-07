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
        START, A, B, C1, C2, C3, C4, C5, D1, D2, E, F, G
    }

    private Beat[] order = { Beat.A, Beat.B, Beat.C1, Beat.C2, Beat.C3, Beat.C4, Beat.C5, Beat.D1, Beat.D2, Beat.E, Beat.F, Beat.G };

    // Start is called before the first frame update
    void Start()
    {
        if (this.storyBeat == Beat.START)
        {
            this.storyBeat = this.order[this.beatIdx];
        } else
        {
            for (uint i = 0; i < order.Length; i++)
            {
                if (order[i] == this.storyBeat)
                {
                    this.beatIdx = i;
                    break;
                }
            }
        }
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

        if (this.storyBeat == Beat.C1)
        {
            if (this.state == State.BEAT_FIRST_UPDATE)
            {
                dm.SetFile("intro");
                dm.StartScene("scene1.5");
            }
            if (this.state == State.PROMPT)
            {
                dm.StartScene("pickupBrown");
            }
            this.state = State.BEAT_ACTIVE;
        }

        if (this.storyBeat == Beat.C2)
        {
            if (this.state == State.PROMPT)
            {
                dm.StartScene("brownInMachine");
            }
            this.state = State.BEAT_ACTIVE;
        }

        if (this.storyBeat == Beat.C3)
        {
            if (this.state == State.PROMPT)
            {
                dm.StartScene("pickupRed");
            }
            this.state = State.BEAT_ACTIVE;
        }

        if (this.storyBeat == Beat.C4)
        {
            if (this.state == State.PROMPT)
            {
                dm.StartScene("redInMachine");
            }
            this.state = State.BEAT_ACTIVE;
        }

        if (this.storyBeat == Beat.C5)
        {
            if (this.state == State.PROMPT)
            {
                dm.StartScene("clickMachine");
            }
            this.state = State.BEAT_ACTIVE;
        }

        if (this.storyBeat == Beat.D1)
        {
            if (this.state == State.BEAT_FIRST_UPDATE)
            {
                dm.SetFile("intro");
                dm.StartScene("toss");
            }
            if (this.state == State.PROMPT)
            {
                dm.StartScene("pickupMonster");
            }
            this.state = State.BEAT_ACTIVE;
        }
        if (this.storyBeat == Beat.D2)
        {
            if (this.state == State.PROMPT)
            {
                dm.StartScene("monsterToVoid");
            }
            this.state = State.BEAT_ACTIVE;
        }        
        
        if (this.storyBeat == Beat.D2)
        {
            if (this.state == State.PROMPT)
            {
                dm.StartScene("monsterToVoid");
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
