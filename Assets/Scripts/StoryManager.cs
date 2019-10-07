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
    private Main main;
    private DialogueManager dm;


    public enum State
    {
        BEAT_FIRST_UPDATE,
        BEAT_ACTIVE,
        PROMPT
    }
    public enum Beat
    {
        START,
        TUTORIAL_PICKUP_ORB,
        TUTORIAL_PICKUP_ORB_INSTRUCTIONS,
        TUTORIAL_DROP_ORB,
        TUTORIAL_DROP_ORB_INSTRUCTIONS,
        TUTORIAL_SUMMON_MONSTER,
        TUTORIAL_RELEASE_MONSTER
    }

    // Start is called before the first frame update
    void Start()
    {
        if (this.storyBeat == Beat.START)
        {
            this.storyBeat = Beat.TUTORIAL_PICKUP_ORB;
        }
        this.beatTimer = 5f;
        this.isBeatTimed = false;
        this.state = State.BEAT_FIRST_UPDATE;

        this.dm = FindObjectOfType<DialogueManager>();
        this.main = FindObjectOfType<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        // Do updates based on story beat.
        if (this.storyBeat == Beat.TUTORIAL_PICKUP_ORB) {
            if (this.state == State.BEAT_FIRST_UPDATE)
            {
                dm.SetFile("intro");
                dm.StartScene("tutorial-pickup-orb");
                this.state = State.BEAT_ACTIVE;
            } else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.GotoBeat(Beat.TUTORIAL_PICKUP_ORB_INSTRUCTIONS);
                }
            }
        } else if (this.storyBeat == Beat.TUTORIAL_PICKUP_ORB_INSTRUCTIONS) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-pickup-orb-instructions", false);
                this.state = State.BEAT_ACTIVE;
            } else if (this.state == State.BEAT_ACTIVE) {
                if (this.main.item != null) {
                    Orb orb = this.main.item.GetComponent<Orb>();
                    if (orb != null) {
                        if (orb.orbColor == Orb.OrbColor.BLUE && this.main.item.state == Item.State.PICKED_UP) {
                            this.GotoBeat(Beat.TUTORIAL_DROP_ORB);
                        }
                    }
                }
            } else if (this.state == State.PROMPT) {
                dm.StartScene("tutorial-pickup-orb-prompt");
                this.state = State.BEAT_ACTIVE;
            }
        } else if (this.storyBeat == Beat.TUTORIAL_DROP_ORB) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-drop-orb");
                this.state = State.BEAT_ACTIVE;
            } else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.GotoBeat(Beat.TUTORIAL_DROP_ORB_INSTRUCTIONS);
                }
            }
        } else if (this.storyBeat == Beat.TUTORIAL_DROP_ORB_INSTRUCTIONS) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-drop-orb-instructions", false);
                this.state = State.BEAT_ACTIVE;
            }
        }
        /*
        if (this.storyBeat == Beat.TUTORIAL_DROP_ORB)
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

        if (this.storyBeat == Beat.E1)
        {
            if (this.state == State.BEAT_FIRST_UPDATE)
            {
                dm.SetFile("intro");
                dm.StartScene("good");
            }
            this.state = State.BEAT_ACTIVE;
        }
        if (this.storyBeat == Beat.E2)
        {
            if (this.state == State.BEAT_FIRST_UPDATE)
            {
                dm.SetFile("intro");
                dm.StartScene("bad");
            }
            this.state = State.BEAT_ACTIVE;
        }
        */
        /*        this.beatTimer -= Time.deltaTime;
                if (this.isBeatTimed && this.beatTimer < 0f) {
                    this.NextBeat();
                }*/
    }

    // Trigger the story beat (can be called from outside when actions done).
    public void GotoBeat(Beat beat) {
        this.storyBeat = beat;
        this.state = State.BEAT_FIRST_UPDATE;
    }

    public void Prompt()
    {
        this.state = State.PROMPT;
    }
}
