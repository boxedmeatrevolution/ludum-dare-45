using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbManager : MonoBehaviour
{
    public Orb[] orbs;
    private bool orbIsPickedUp;
    private Orb pickedUpOrb;

    private List<Orb> orbsInMachine;
    private List<Orb> orbsInMonster;

    private Stack<Orb> inactiveGhostOrbs = new Stack<Orb>();

    // Start is called before the first frame update
    void Start()
    {
        this.orbIsPickedUp = false;
        this.orbs = GetComponentsInChildren<Orb>();

        for (int i = 0; i < this.orbs.Length; i++)
        {
            if (this.orbs[i].orbColor == Orb.OrbColor.WHITE)
            {
                this.inactiveGhostOrbs.Push(this.orbs[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // determine if any orb is picked up
        this.orbIsPickedUp = false;
        for (int i = 0; i < this.orbs.Length; i++)
        {
            Orb orb = this.orbs[i];
            if (orb.item.pickedUp)
            {
                this.orbIsPickedUp = true;
                this.pickedUpOrb = orb;
                break;
            }
        }
    }

    public bool IsOrbPickedUp()
    {
        return this.orbIsPickedUp;
    }

    public Orb GetPickedUpOrb()
    {
        return this.pickedUpOrb;
    }

    public void MakeGhostOrb(Orb primaryOrb, Orb[] otherOrbs)
    {
        if (this.inactiveGhostOrbs.Count < 0) {
            return;
        }

        StoryManager storyManager = FindObjectOfType<StoryManager>();
        if (storyManager.storyBeat == StoryManager.Beat.FREE_PLAY_1) {
            storyManager.GotoBeat(StoryManager.Beat.INTERLUDE_2_PABS_EMERGE, 2f);
        }

        Orb ghostOrb = this.inactiveGhostOrbs.Pop();
        ghostOrb.AddChildOrb(primaryOrb);
        for (int i = 0; i < otherOrbs.Length; i++)
        {
            ghostOrb.AddChildOrb(otherOrbs[i]);
        }

        primaryOrb.item.state = Item.State.TRANSFORMED;
        for (int i = 0; i < otherOrbs.Length; i++)
        {
            otherOrbs[i].item.state = Item.State.TRANSFORMED;
        }

        ghostOrb.item.transform.position = primaryOrb.transform.position;
        ghostOrb.item.initialPosition = ghostOrb.item.transform.position;

    }
}
