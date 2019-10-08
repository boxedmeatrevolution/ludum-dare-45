using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingFlame : Monster {
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    public override void OnDying() {
        this.GetComponentInChildren<Animator>().Play("LivingFlame_Dead");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override bool CanBurn() {
        return false;
    }

    public override bool IsFireElemental() {
        return true;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected override State ChooseThreatenOffensive(Monster other) {
        if (other is PlantOgre || other is Goblin || other is Copycat) {
            return State.THREATEN;
        } else if (other is GhostSlug || other is Ghost) {
            return State.FLEE;
        } else {
            return State.IGNORE;
        }
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster threatens.
    protected override State ChooseThreatenDefensive(Monster other) {
        return this.ChooseThreatenOffensive(other);
    }

    // Choose whether you survive the fight with the other monster.
    public override bool KillOpponent(Monster other) {
        if (other is FireSalamander || other is GhostSlug || other is Goblin || other is LivingFlame) {
            return true;
        }
        return false;
    }
}
