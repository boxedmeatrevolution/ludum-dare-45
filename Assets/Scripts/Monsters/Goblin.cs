using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster {
    private static uint goblinIndex = 0;
    private uint myGoblinIndex;
    private Animator animator;
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        this.animator = GetComponentInChildren<Animator>();
        Goblin.goblinIndex += 1;
        Goblin.goblinIndex = Goblin.goblinIndex % 3;
        this.myGoblinIndex = Goblin.goblinIndex;
        this.animator.Play("Goblin_" + this.myGoblinIndex.ToString());
    }

    public override void OnDying() {
        this.GetComponentInChildren<Animator>().Play("Goblin_" + this.myGoblinIndex.ToString() + "_Dead");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected override State ChooseThreatenOffensive(Monster other) {
        return State.FLEE;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster threatens.
    protected override State ChooseThreatenDefensive(Monster other) {
        return State.FLEE;
    }

    // Choose whether you survive the fight with the other monster.
    public override bool KillOpponent(Monster other) {
        if (other is GhostSlug || other is FireSalamander || other is Goblin) {
            return true;
        }
        return false;
    }
}
