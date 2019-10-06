﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSalamander : Monster {
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

    public override bool IsFirey() {
        return true;
    }

    public override bool IsExplosive() {
        return true;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected override State ChooseThreatenOffensive(Monster other) {
        return State.WANDER;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster threatens.
    protected override State ChooseThreatenDefensive(Monster other) {
        return State.WANDER;
    }

    // Choose whether you survive the fight with the other monster.
    protected override bool SurviveFight(Monster other) {
        return false;
    }
}
