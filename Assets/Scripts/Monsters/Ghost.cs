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
