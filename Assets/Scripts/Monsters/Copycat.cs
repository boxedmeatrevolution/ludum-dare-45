using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Copycat : Monster {
    private readonly static float COPYCAT_TIME = 12f;
    private Dictionary<Type, float> copycatProcess;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        this.copycatProcess = new Dictionary<Type, float>();
    }

    public override void OnDying() {
        this.GetComponentInChildren<Animator>().Play("Copycat_Dead");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        if (this.GetItem().state == Item.State.ON_GROUND && this.state == State.WANDER) {
            Monster minMonster = null;
            float minDistance = float.PositiveInfinity;
            foreach (Monster monster in FindObjectsOfType<Monster>()) {
                if (monster.Initialized() && monster != this && monster.state != State.DEAD && monster.state != State.DYING) {
                    float distance = ((Vector2)monster.transform.position - (Vector2)this.transform.position).magnitude;
                    if (distance < minDistance) {
                        minDistance = distance;
                        minMonster = monster;
                    }
                }
            }
            if (minMonster != null) {
                Type monsterType = minMonster.GetType();
                if (!this.copycatProcess.ContainsKey(monsterType)) {
                    this.copycatProcess.Add(monsterType, 0f);
                }
                this.copycatProcess[monsterType] += Time.deltaTime;
                if (this.copycatProcess[monsterType] > Copycat.COPYCAT_TIME) {
                    GameObject prefab = PrefabManager.GetMonsterPrefab(monsterType);
                    Monster monster = Instantiate(prefab, this.transform.position, Quaternion.identity).GetComponent<Monster>();
                    monster.SetCopycat(true);
                    monster.orbs = this.orbs;
                    Destroy(this.gameObject);
                }
                if (UnityEngine.Random.value > 1 - Time.deltaTime / 0.5f) {
                    GameObject particleObj = Instantiate(PrefabManager.COPYCAT_PARTICLE_PREFAB, minMonster.transform.position, Quaternion.identity);
                    Particle particle = particleObj.GetComponent<Particle>();
                    particle.target = this.transform.position;
                }
            }
        }
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected override State ChooseThreatenOffensive(Monster other) {
        if (other is Ghost) {
            return State.FLEE;
        }
        else if (other is GhostSlug || other is FireSalamander || other is Goblin) {
            return State.THREATEN;
        }
        else {
            return State.IGNORE;
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
        if (other is PlantOgre || other is Ghost) {
            return false;
        }
        return true;
    }
}
