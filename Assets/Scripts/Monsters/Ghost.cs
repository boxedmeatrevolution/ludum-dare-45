using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Monster {
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        if (this.state == State.TRAVEL_TO_TRANSFORM_ORB && this.transformOrb.item.state != Item.State.ON_GROUND)
        {
            this.state = State.WANDER;
        }

        if (this.state == State.WANDER)
        {
            foreach (Orb orb in FindObjectsOfType<Orb>())
            {
                if (orb.item.state == Item.State.ON_GROUND && this.IsPositionInPen(orb.transform.position))
                {
                    this.state = State.TRAVEL_TO_TRANSFORM_ORB;
                    this.transformOrb = orb;
                    break;
                }
            }
        }

        base.Update();
    }

    public override bool CanBurn() {
        return false;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected override State ChooseThreatenOffensive(Monster other) {
        return State.LURE;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster threatens.
    protected override State ChooseThreatenDefensive(Monster other) {
        return this.ChooseThreatenOffensive(other);
    }

    // Choose whether you survive the fight with the other monster.
    public override bool KillOpponent(Monster other) {
        return false;
    }

    public override bool KillDigestedOpponent(Monster other) {
        return true;
    }
}
