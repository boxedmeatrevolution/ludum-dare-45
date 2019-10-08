using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSlug : Monster {

    private Monster fireTarget;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    public override void OnDying() {
        this.GetComponentInChildren<Animator>().Play("GhostSlug_Dead");
    }

    // Update is called once per frame
    protected override void Update() {
        if (this.state == State.TRAVEL_TO_TRANSFORM_ORB && (!this.transformOrb.enflamed || this.transformOrb.item.state != Item.State.ON_GROUND))
        {
            this.state = State.WANDER;
        }
        
        if (this.state == State.WANDER) {
            foreach (Orb orb in FindObjectsOfType<Orb>())
            {
                if (orb.enflamed && orb.item.state == Item.State.ON_GROUND && this.IsPositionInPen(orb.transform.position))
                {
                    this.state = State.TRAVEL_TO_TRANSFORM_ORB;
                    this.transformOrb = orb;
                    break;
                }
            }
            if (this.fireTarget == null || !this.fireTarget.IsFiery()) {
                this.ChooseFieryTarget();
            }
            GameObject chasingTarget = null;
            if (this.transformOrb != null) {
                chasingTarget = this.transformOrb.gameObject;
            }
            if (chasingTarget == null && this.fireTarget != null) {
                chasingTarget = this.fireTarget.gameObject;
            }
            if (chasingTarget != null) {
                if (Random.value > 1f - Time.deltaTime / 1f) {
                    GameObject particleObj = Instantiate(PrefabManager.FIRE_PARTICLE_PREFAB, chasingTarget.transform.position, Quaternion.identity);
                    Particle particle = particleObj.GetComponent<Particle>();
                    particle.target = this.transform.position;
                }
            }
        }

        base.Update();
    }

    public void ChooseFieryTarget() {
        this.fireTarget = null;
        foreach (Monster monster in FindObjectsOfType<Monster>()) {
            if (!monster.Initialized()) {
                continue;
            }
            if (monster.IsFiery()) {
                this.fireTarget = monster;
                break;
            }
        }
    }

    public override Vector2 ChooseWaypoint(Pen pen) {
        if (this.fireTarget != null) {
            return this.fireTarget.transform.position;
        }
        else {
            return base.ChooseWaypoint(pen);
        }
    }

    public override bool CanBurn() {
        return false;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected override State ChooseThreatenOffensive(Monster other) {
        if (other.IsFiery()) {
            return State.LURE;
        }
        return State.IGNORE;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster threatens.
    protected override State ChooseThreatenDefensive(Monster other) {
        return this.ChooseThreatenOffensive(other);
    }

    // Choose whether you survive the fight with the other monster.
    public override bool KillOpponent(Monster other) {
        return false;
    }
}
