using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    protected enum State {
        Wander,
        Encounter,
        Threaten,
        PreFight,
        Fight,
        PostFight,
        Panic,
        Dead
    }
    private Pen pen;
    private Vector2 waypoint;
    protected State state;
    protected Monster fightTarget;
    protected float fightTimer;
    protected Monster panicTarget;
    protected Vector2 panicVel;
    protected Monster threatenTarget;
    protected float threatenTimer;
    protected Vector2 fightVel;
    public float encounterTime = 1f;
    public float averageWaitTime = 4f;
    public float speed = 0.35f;
    public float panicSpeed = 0.8f;
    public float panicAccel = 2f;
    public float fightSpeed = 2f;
    public float fightAccel = 2f;
    public float aggressionRadius = 2f;
    public float panicRadius = 4f;
    public float fightTime = 2f;
    public float threatenTime = 2f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.pen = FindObjectOfType<Pen>();
        this.state = State.Wander;
        this.ChooseWaypoint();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (this.state == State.Wander) {
            Vector2 displacement = this.waypoint - (Vector2)this.transform.position;
            if (displacement.magnitude > 0.01) {
                Vector2 direction = displacement.normalized;
                Vector2 proposedMove = (Vector3)direction * speed * Time.deltaTime;
                if (this.panicTarget != null) {
                    Monster monster = this.panicTarget;
                    Vector2 panic_displacement = this.transform.position - monster.transform.position;
                    if (panic_displacement.magnitude != 0 && panic_displacement.magnitude < this.panicRadius) {
                        float correction = -Vector2.Dot(proposedMove, panic_displacement) / panic_displacement.magnitude;
                        if (correction > 0f) {
                            proposedMove += correction * panic_displacement / panic_displacement.magnitude;
                        }
                        if (proposedMove.magnitude != 0) {
                            proposedMove *= this.speed * Time.deltaTime / proposedMove.magnitude;
                        }
                    }
                }
                this.transform.position += (Vector3)proposedMove;
            } else if (Random.value > (1 - (Time.deltaTime / this.averageWaitTime))) {
                this.ChooseWaypoint();
            }
            
            foreach (Monster monster in FindObjectsOfType<Monster>()) {
                if (monster == this) {
                    continue;
                }
                if (monster.state == State.Wander) {
                    Vector2 monster_displacement = monster.transform.position - this.transform.position;
                    if (monster_displacement.magnitude < this.aggressionRadius) {
                        this.state = this.ChooseThreatenOffensive(monster);
                        monster.state = monster.ChooseThreatenOffensive(this);
                        if (monster.state == State.Threaten && this.state == State.Wander) {
                            this.state = this.ChooseThreatenDefensive(monster);
                        }
                        if (this.state == State.Threaten && monster.state == State.Wander) {
                            monster.state = monster.ChooseThreatenDefensive(this);
                        }
                        if (this.state == State.Threaten) {
                            this.threatenTarget = monster;
                            this.threatenTimer = this.threatenTime;
                        }
                        if (monster.state == State.Threaten) {
                            monster.threatenTarget = this;
                            monster.threatenTimer = monster.threatenTime;
                        }
                        if (this.state == State.Panic) {
                            this.panicTarget = monster;
                            this.panicVel = new Vector2();
                        }
                        if (monster.state == State.Panic) {
                            monster.panicTarget = this;
                            monster.panicVel = new Vector2();
                        }
                    }
                }
            }
        }
        if (this.state == State.Threaten) {
            this.threatenTimer -= Time.deltaTime;
            if (this.threatenTimer < 0f) {
                Monster monster = this.threatenTarget;
                float distance = (monster.transform.position - this.transform.position).magnitude;
                if (this.threatenTarget.state == State.Threaten && this.threatenTarget.threatenTarget == this) {
                    this.fightTarget = monster;
                    this.state = State.PreFight;
                    this.fightVel = new Vector2();
                    monster.fightTarget = this;
                    monster.state = State.PreFight;
                    monster.fightVel = new Vector2();
                } else if (distance > this.panicRadius && distance > monster.panicRadius) {
                    this.state = State.Wander;
                }
            }
        }
        if (this.state == State.PreFight) {
            // Charge at one another.
            Monster monster = this.threatenTarget;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (displacement.magnitude > 0.1) {
                this.fightVel += displacement.normalized * this.fightAccel * Time.deltaTime;
                if (this.fightVel.magnitude > this.fightSpeed) {
                    this.fightVel *= this.fightSpeed / this.fightVel.magnitude;
                }
                this.transform.position += (Vector3)this.fightVel * Time.deltaTime;
            } else {
                this.state = State.Fight;
                this.fightTimer = this.fightTime;
                monster.state = State.Fight;
                monster.fightTimer = monster.fightTime;
            }
        }
        if (this.state == State.Fight) {
            Monster monster = this.fightTarget;
            this.transform.position = monster.transform.position;
            this.fightTimer -= Time.deltaTime;
            if (this.fightTimer < 0f && monster.fightTimer < 0f) {
                if (!this.SurviveFight(monster)) {
                    this.state = State.Dead;
                    monster.fightTarget = null;
                } else {
                    this.state = State.PostFight;
                    this.transform.position += new Vector3(
                        Random.Range(-0.1f, 0.1f),
                        Random.Range(-0.1f, 0.1f));
                }
                if (!monster.SurviveFight(this)) {
                    monster.state = State.Dead;
                    this.fightTarget = null;
                } else {
                    monster.state = State.PostFight;
                    monster.transform.position += new Vector3(
                        Random.Range(-0.1f, 0.1f),
                        Random.Range(-0.1f, 0.1f));
                }
            }
        }
        if (this.state == State.PostFight) {
            Monster monster = this.fightTarget;
            if (monster == null) {
                this.state = State.Wander;
            } else {
                Vector2 displacement = monster.transform.position - this.transform.position;
                if (displacement.magnitude != 0f) {
                    this.transform.position -= (Vector3)displacement / displacement.magnitude * this.speed * Time.deltaTime;
                }
                if (displacement.magnitude > this.panicRadius && displacement.magnitude > monster.panicRadius) {
                    this.state = State.Wander;
                    monster.state = State.Wander;
                }
            }
        }
        if (this.state == State.Panic) {
            // Charge away from one another.
            Monster monster = this.panicTarget;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (displacement.magnitude > this.panicRadius && displacement.magnitude > monster.panicRadius) {
                if (this.panicVel.magnitude < this.speed) {
                    this.state = State.Wander;
                }
                else {
                    this.panicVel -= this.panicAccel * this.panicVel.normalized * Time.deltaTime;
                }
            } else if (displacement.magnitude > 0.01) {
                this.panicVel -= displacement.normalized * this.panicAccel * Time.deltaTime;
                if (this.panicVel.magnitude > this.panicSpeed) {
                    this.panicVel *= this.panicSpeed / this.panicVel.magnitude;
                }
            }
            this.transform.position += (Vector3)this.panicVel * Time.deltaTime;
            /*
            // OLD
            Monster monster = this.panicTarget;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (displacement.magnitude != 0f) {
                this.transform.position -= (Vector3)displacement / displacement.magnitude * this.panicSpeed * Time.deltaTime;
            }
            if (displacement.magnitude > this.panicRadius && displacement.magnitude > monster.panicRadius) {
                this.state = State.Wander;
                monster.state = State.Wander;
            }*/
        }
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected virtual State ChooseThreatenOffensive(Monster other) {
        return State.Wander;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster threatens.
    protected virtual State ChooseThreatenDefensive(Monster other) {
        return State.Panic;
    }
    
    // Choose whether you survive the fight with the other monster.
    protected virtual bool SurviveFight(Monster other) {
        return true;
    }

    private void ChooseWaypoint() {
        this.waypoint = (Vector2)this.pen.transform.position + new Vector2(
            Random.Range(0, pen.width),
            Random.Range(0, pen.height));
    }
}
