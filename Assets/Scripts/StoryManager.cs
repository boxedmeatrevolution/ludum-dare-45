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

    private StoryCharacter ghostighost;
    private StoryCharacter pabs;
    private MusicManager musicManager;


    public enum State
    {
        BEAT_FIRST_UPDATE,
        BEAT_ACTIVE,
        PROMPT
    }
    public enum Beat
    {
        START,
        TUTORIAL_GHOSTIGHOST_EMERGE,
        TUTORIAL_PICKUP_ORB,
        TUTORIAL_PICKUP_ORB_INSTRUCTIONS,
        TUTORIAL_DROP_ORB,
        TUTORIAL_DROP_ORB_INSTRUCTIONS,
        TUTORIAL_SUMMON_MONSTER,
        TUTORIAL_SUMMON_MONSTER_INSTRUCTIONS,
        TUTORIAL_RELEASE_MONSTER,
        TUTORIAL_RELEASE_MONSTER_INSTRUCTIONS,
        TUTORIAL_SALAMANDER,
        TUTORIAL_SALAMANDER_ESCAPE,
        TUTORIAL_ENDING,
        FREE_PLAY_1,
        INTERLUDE_1_PABS_EMERGE,
        INTERLUDE_1_PABS_TALK,
        FREE_PLAY_2,
        INTERLUDE_2_PABS_EMERGE,
        INTERLUDE_2_PABS_TALK,
        FREE_PLAY_3,
        INTERLUDE_3_PABS_TALK,
        FREE_PLAY_4,
        ENDING_PABS_EMERGE,
        ENDING_PABS_TALK,
        FREE_PLAY_5
    }

    // Start is called before the first frame update
    void Start() {
        this.dm = FindObjectOfType<DialogueManager>();
        this.main = FindObjectOfType<Main>();
        if (this.storyBeat == Beat.START)
        {
            this.storyBeat = Beat.TUTORIAL_GHOSTIGHOST_EMERGE;
            this.beatTimer = 1.25f;
            this.state = State.BEAT_FIRST_UPDATE;
        }
        this.beatTimer = 5f;
        this.isBeatTimed = false;
        this.state = State.BEAT_FIRST_UPDATE;
        this.musicManager = FindObjectOfType<MusicManager>();
        this.musicManager.PlayGhostighostTheme();
    }

    // Update is called once per frame
    void Update() {
        this.beatTimer -= Time.deltaTime;
        // Do updates based on story beat.
        if (this.storyBeat == Beat.TUTORIAL_GHOSTIGHOST_EMERGE) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.musicManager.PlayGhostighostTheme();
                this.ghostighost = Instantiate(PrefabManager.GHOSTIGHOST_PREFAB).GetComponent<StoryCharacter>();
                this.state = State.BEAT_ACTIVE;
            }
            if (this.beatTimer < 0f) {
                this.GotoBeat(Beat.TUTORIAL_PICKUP_ORB);
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_PICKUP_ORB) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-pickup-orb");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.GotoBeat(Beat.TUTORIAL_PICKUP_ORB_INSTRUCTIONS);
                }
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_PICKUP_ORB_INSTRUCTIONS) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-pickup-orb-instructions", false);
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (this.main.item != null) {
                    Orb orb = this.main.item.GetComponent<Orb>();
                    if (orb != null) {
                        if (orb.orbColor == Orb.OrbColor.BLUE && this.main.item.state == Item.State.PICKED_UP) {
                            this.GotoBeat(Beat.TUTORIAL_DROP_ORB);
                        }
                    }
                }
            }
            else if (this.state == State.PROMPT) {
                dm.StartScene("tutorial-pickup-orb-prompt");
                this.state = State.BEAT_ACTIVE;
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_DROP_ORB) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-drop-orb");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.GotoBeat(Beat.TUTORIAL_DROP_ORB_INSTRUCTIONS);
                }
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_DROP_ORB_INSTRUCTIONS) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-drop-orb-instructions", false);
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                bool allOnGround = true;
                foreach (Orb orb in FindObjectsOfType<Orb>()) {
                    Item item = orb.GetComponent<Item>();
                    if (item.state != Item.State.ON_GROUND) {
                        allOnGround = false;
                        break;
                    }
                }
                if (allOnGround) {
                    this.GotoBeat(Beat.TUTORIAL_SUMMON_MONSTER);
                }
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_SUMMON_MONSTER) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-summon-monster");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.GotoBeat(Beat.TUTORIAL_SUMMON_MONSTER_INSTRUCTIONS);
                }
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_SUMMON_MONSTER_INSTRUCTIONS) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-summon-monster-instructions", false);
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                FireSalamander salamander = FindObjectOfType<FireSalamander>();
                if (salamander != null) {
                    if (salamander.state == Monster.State.WANDER) {
                        this.GotoBeat(Beat.TUTORIAL_RELEASE_MONSTER);
                    }
                }
            }
            else if (this.state == State.PROMPT) {
                dm.StartScene("tutorial-summon-monster-prompt");
                this.state = State.BEAT_ACTIVE;
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_RELEASE_MONSTER) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-release-monster");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.GotoBeat(Beat.TUTORIAL_RELEASE_MONSTER_INSTRUCTIONS);
                }
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_RELEASE_MONSTER_INSTRUCTIONS) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-release-monster-instructions", false);
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                FireSalamander salamander = FindObjectOfType<FireSalamander>();
                if (salamander == null) {
                    this.GotoBeat(Beat.TUTORIAL_SALAMANDER);
                }
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_SALAMANDER) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-salamander");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.GotoBeat(Beat.TUTORIAL_SALAMANDER_ESCAPE);
                }
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_SALAMANDER_ESCAPE) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-salamander-escape", false);
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                bool allSalamanders = true;
                Monster[] monsters = FindObjectsOfType<Monster>();
                foreach (Monster monster in monsters) {
                    if (!(monster is FireSalamander)) {
                        allSalamanders = false;
                        break;
                    }
                }
                if (!allSalamanders) {
                    this.GotoBeat(Beat.TUTORIAL_ENDING);
                }
            }
        }
        else if (this.storyBeat == Beat.TUTORIAL_ENDING) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("intro");
                dm.StartScene("tutorial-ending");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.ghostighost.ReturnToVoid();
                    this.ghostighost = null;
                    this.GotoBeat(Beat.FREE_PLAY_1);
                }
            }
        }
        else if (this.storyBeat == Beat.FREE_PLAY_1) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.musicManager.PlayMainTheme();
                this.state = State.BEAT_ACTIVE;
            }
        }
        else if (this.storyBeat == Beat.INTERLUDE_1_PABS_EMERGE) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.musicManager.PlayPabsTheme();
                this.state = State.BEAT_ACTIVE;
                this.pabs = Instantiate(PrefabManager.PABS_PREFAB).GetComponent<StoryCharacter>();
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (this.beatTimer < 0f) {
                    this.GotoBeat(Beat.INTERLUDE_1_PABS_TALK);
                }
            }
        }
        else if (this.storyBeat == Beat.INTERLUDE_1_PABS_TALK) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("goblin_encounter_1");
                dm.StartScene("interlude1-pabs-talk");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.pabs.ReturnToVoid();
                    this.pabs = null;
                    this.GotoBeat(Beat.FREE_PLAY_2);
                }
            }
        }
        else if (this.storyBeat == Beat.FREE_PLAY_2) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.musicManager.PlayMainTheme();
                this.state = State.BEAT_ACTIVE;
            }
        }
        else if (this.storyBeat == Beat.INTERLUDE_2_PABS_EMERGE) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.musicManager.PlayPabsTheme();
                this.state = State.BEAT_ACTIVE;
                this.pabs = Instantiate(PrefabManager.PABS_PREFAB).GetComponent<StoryCharacter>();
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (this.beatTimer < 0f) {
                    this.GotoBeat(Beat.INTERLUDE_2_PABS_TALK);
                }
            }
        }
        else if (this.storyBeat == Beat.INTERLUDE_2_PABS_TALK) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("win_encounter");
                dm.StartScene("ghost_orb_created");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.GotoBeat(Beat.FREE_PLAY_3);
                }
            }
        }
        else if (this.storyBeat == Beat.FREE_PLAY_3) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.state = State.BEAT_ACTIVE;
            }
            if (this.state == State.BEAT_ACTIVE) {
                Ghost ghost = FindObjectOfType<Ghost>();
                if (ghost != null) {
                    this.GotoBeat(Beat.INTERLUDE_3_PABS_TALK);
                }
            }
        }
        else if (this.storyBeat == Beat.INTERLUDE_3_PABS_TALK) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("win_encounter");
                dm.StartScene("one_orb_to_summon");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.pabs.ReturnToVoid();
                    this.pabs = null;
                    this.GotoBeat(Beat.FREE_PLAY_4);
                }
            }
        }
        else if (this.storyBeat == Beat.FREE_PLAY_4) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.musicManager.PlayMainTheme();
                this.state = State.BEAT_ACTIVE;
            }
        }
        else if (this.storyBeat == Beat.ENDING_PABS_EMERGE) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.musicManager.PlayPabsTheme();
                this.state = State.BEAT_ACTIVE;
                this.pabs = Instantiate(PrefabManager.PABS_PREFAB).GetComponent<StoryCharacter>();
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (this.beatTimer < 0f) {
                    this.GotoBeat(Beat.ENDING_PABS_TALK);
                }
            }
        }
        else if (this.storyBeat == Beat.ENDING_PABS_TALK) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                dm.SetFile("win_encounter");
                dm.StartScene("win_scene");
                this.state = State.BEAT_ACTIVE;
            }
            else if (this.state == State.BEAT_ACTIVE) {
                if (dm.endOfScene) {
                    this.pabs.ReturnToVoid();
                    this.pabs = null;
                    this.GotoBeat(Beat.FREE_PLAY_5);
                }
            }
        }
        else if (this.storyBeat == Beat.FREE_PLAY_5) {
            if (this.state == State.BEAT_FIRST_UPDATE) {
                this.musicManager.PlayGhostighostTheme();
                this.state = State.BEAT_ACTIVE;
            }
        }
    }

    // Trigger the story beat (can be called from outside when actions done).
    public void GotoBeat(Beat beat, float timer = 0f) {
        this.dm.EndScene();
        this.storyBeat = beat;
        this.beatTimer = timer;
        this.state = State.BEAT_FIRST_UPDATE;
    }

    public void Prompt()
    {
        this.state = State.PROMPT;
    }
}
