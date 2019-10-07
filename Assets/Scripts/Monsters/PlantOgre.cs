using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantOgre : Monster {
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    public override void OnDying() {
        this.GetComponentInChildren<Animator>().Play("PlantOgre_Dead");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected override State ChooseThreatenOffensive(Monster other) {
        if (other is PlantOgre) {
            return State.WANDER;
        } if (other is GhostSlug) {
            return State.FLEE;
        } else {
            return State.THREATEN;
        }
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster threatens.
    protected override State ChooseThreatenDefensive(Monster other) {
        return this.ChooseThreatenOffensive(other);
    }

    // Choose whether you survive the fight with the other monster.
    /*protected override bool SurviveFight(Monster other) {
        return true;
    }*/
    public override bool KillOpponent(Monster other) {
        if (other is PlantOgre) {
            return false;
        }
        return true;
    }
}
