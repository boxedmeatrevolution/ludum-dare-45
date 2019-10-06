using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public enum State {
        WANDER,
        THREATEN,
        PRE_FIGHT,
        FIGHT,
        POST_FIGHT,
        PANIC,
        DEAD
    }
    private Emotion emotion;
    private SpriteRenderer renderer;
    private Pen pen;
    private Vector2 waypoint;
    private Fire fire;
    protected State state;
    protected float postFightTimer;
    protected Monster fightTarget;
    protected float fightTimer;
    protected float avoidTimer;
    protected Monster panicTarget;
    protected Vector2 panicVel;
    protected Monster threatenTarget;
    protected float threatenTimer;
    protected Vector2 fightVel;
    protected Vector2 deadVel;
    protected Item item;
    protected bool isThreatened;
    protected FightCloud fightCloud;
    protected bool dead;
    protected bool enflamed;
    public float fireHeight = 0.2f;
    public bool canPickUp = true;
    public float averageWaitTime = 4f;
    public float postFightTime = 1f;
    public float speed = 0.35f;
    public float avoidTime = 10f;
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
        this.renderer = GetComponentInChildren<SpriteRenderer>();
        this.emotion = GetComponentInChildren<Emotion>();
        this.pen = FindObjectOfType<Pen>();
        this.state = State.WANDER;
        this.isThreatened = false;
        this.item = GetComponent<Item>();
        this.fightCloud = null;
        this.enflamed = false;
        this.ChooseWaypoint();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (this.enflamed) {
            if (this.fire == null) {
                GameObject fireObj = Instantiate(PrefabManager.FIRE_PREFAB, this.renderer.transform);
                this.fire = fireObj.GetComponentInChildren<Fire>();
                this.fire.transform.localPosition = new Vector2(0, this.fireHeight - this.renderer.transform.localPosition.y);
            }
        } else {
            if (this.fire != null) {
                this.fire.FireOver();
                this.fire = null;
            }
        }
        this.emotion.UpdateFromState(this.state);
        if (this.item.state == Item.State.ON_GROUND) {
            if (this.state == State.WANDER) {
                Vector2 displacement = this.waypoint - (Vector2)this.transform.position;
                if (displacement.magnitude > 0.01) {
                    Vector2 direction = displacement.normalized;
                    Vector2 proposedMove = (Vector3)direction * speed * Time.deltaTime;
                    if (this.panicTarget != null) {
                        this.avoidTimer -= Time.deltaTime;
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
                    if (this.avoidTimer < 0f) {
                        this.panicTarget = null;
                    }
                }
                else if (Random.value > (1 - (Time.deltaTime / this.averageWaitTime))) {
                    this.ChooseWaypoint();
                }

                foreach (Monster monster in FindObjectsOfType<Monster>()) {
                    if (monster == this) {
                        continue;
                    }
                    if (monster.state == State.WANDER && !monster.isThreatened && monster.item.state == Item.State.ON_GROUND) {
                        Vector2 monster_displacement = monster.transform.position - this.transform.position;
                        if (monster_displacement.magnitude < this.aggressionRadius) {
                            this.state = this.ChooseThreatenOffensive(monster);
                            monster.state = monster.ChooseThreatenOffensive(this);
                            if (monster.state == State.THREATEN && this.state == State.WANDER) {
                                this.state = this.ChooseThreatenDefensive(monster);
                            }
                            if (this.state == State.THREATEN && monster.state == State.WANDER) {
                                monster.state = monster.ChooseThreatenDefensive(this);
                            }
                            if (this.state == State.THREATEN) {
                                monster.isThreatened = true;
                                this.threatenTarget = monster;
                                this.threatenTimer = this.threatenTime;
                            }
                            if (monster.state == State.THREATEN) {
                                this.isThreatened = true;
                                monster.threatenTarget = this;
                                monster.threatenTimer = monster.threatenTime;
                            }
                            if (this.state == State.PANIC) {
                                this.panicTarget = monster;
                                this.panicVel = Vector2.zero;
                            }
                            if (monster.state == State.PANIC) {
                                monster.panicTarget = this;
                                monster.panicVel = Vector2.zero;
                            }
                        }
                    }
                    if (this.isThreatened || this.state != State.WANDER) {
                        break;
                    }
                }
            }
            if (this.state == State.THREATEN) {
                this.threatenTimer -= Time.deltaTime;
                if (this.threatenTimer < 0f) {
                    Monster monster = this.threatenTarget;
                    float distance = (monster.transform.position - this.transform.position).magnitude;
                    if (this.threatenTarget.state == State.THREATEN && this.threatenTarget.threatenTarget == this) {
                        this.fightTarget = monster;
                        this.state = State.PRE_FIGHT;
                        this.fightVel = Vector2.zero;
                        if (monster.state == State.THREATEN) {
                            monster.fightTarget = this;
                            monster.state = State.PRE_FIGHT;
                            monster.fightVel = Vector2.zero;
                        }
                    }
                    else if (distance > this.panicRadius && distance > monster.panicRadius) {
                        this.state = State.WANDER;
                        this.threatenTarget.isThreatened = false;
                    }
                }
            }
            if (this.state == State.PRE_FIGHT) {
                // Charge at one another.
                Monster monster = this.threatenTarget;
                Vector2 displacement = monster.transform.position - this.transform.position;
                if (displacement.magnitude > 0.1) {
                    this.fightVel += displacement.normalized * this.fightAccel * Time.deltaTime;
                    if (this.fightVel.magnitude > this.fightSpeed) {
                        this.fightVel *= this.fightSpeed / this.fightVel.magnitude;
                    }
                    this.transform.position += (Vector3)this.fightVel * Time.deltaTime;
                }
                else {
                    this.state = State.FIGHT;
                    this.fightTimer = this.fightTime;
                    monster.state = State.FIGHT;
                    monster.fightTimer = monster.fightTime;
                    if (this.IsFirey() || this.enflamed) {
                        if (monster.CanBurn()) {
                            monster.Enflame();
                        }
                    }
                    if (monster.IsFirey() || monster.enflamed) {
                        if (this.CanBurn()) {
                            this.Enflame();
                        }
                    }
                }
            }
            if (this.state == State.FIGHT) {
                Monster monster = this.fightTarget;
                this.transform.position = monster.transform.position;
                this.fightTimer -= Time.deltaTime;
                if (this.fightCloud == null) {
                    GameObject fightCloudObj = Instantiate(PrefabManager.FIGHT_CLOUD_PREFAB, this.transform.position, Quaternion.identity);
                    this.fightCloud = fightCloudObj.GetComponent<FightCloud>();
                    monster.fightCloud = this.fightCloud;
                    this.fightCloud.fighter1 = this;
                    this.fightCloud.fighter2 = monster;
                }
                if (this.fightTimer < 0f && monster.fightTimer < 0f) {
                    this.fightCloud.FightOver();
                    this.fightCloud = null;
                    monster.fightCloud = null;
                    if (!this.SurviveFight(monster)) {
                        this.state = State.DEAD;
                        this.deadVel = new Vector2(
                            Random.Range(1f, 2f),
                            Random.Range(-1f, 1f));
                        monster.fightTarget = null;
                    }
                    else {
                        this.state = State.POST_FIGHT;
                        this.postFightTimer = this.postFightTime;
                        this.transform.position += new Vector3(
                            Random.Range(-0.1f, 0.1f),
                            Random.Range(-0.1f, 0.1f));
                    }
                    if (!monster.SurviveFight(this)) {
                        monster.state = State.DEAD;
                        monster.deadVel = new Vector2(
                            Random.Range(-2f, -1f),
                            Random.Range(-1f, 1f));
                        this.fightTarget = null;
                    }
                    else {
                        monster.state = State.POST_FIGHT;
                        monster.postFightTimer = monster.postFightTime;
                        monster.transform.position += new Vector3(
                            Random.Range(-0.1f, 0.1f),
                            Random.Range(-0.1f, 0.1f));
                    }
                }
            }
            if (this.state == State.POST_FIGHT) {
                Monster monster = this.fightTarget;
                this.postFightTimer -= Time.deltaTime;
                if (monster == null) {
                    if (this.postFightTimer < 0f) {
                        this.isThreatened = false;
                        this.state = State.WANDER;
                    }
                }
                else {
                    Vector2 displacement = monster.transform.position - this.transform.position;
                    if (displacement.magnitude != 0f) {
                        this.transform.position -= (Vector3)displacement / displacement.magnitude * this.speed * Time.deltaTime;
                    }
                    if (displacement.magnitude > this.panicRadius && displacement.magnitude > monster.panicRadius) {
                        this.isThreatened = false;
                        monster.isThreatened = false;
                        this.state = State.WANDER;
                        monster.state = State.WANDER;
                    }
                }
            }
            if (this.state == State.PANIC) {
                // Charge away from one another.
                Monster monster = this.panicTarget;
                Vector2 displacement = monster.transform.position - this.transform.position;
                if (displacement.magnitude > this.panicRadius && displacement.magnitude > monster.panicRadius) {
                    if (this.panicVel.magnitude < this.speed) {
                        if (monster.state == State.THREATEN && monster.threatenTarget == this) {
                            monster.threatenTarget = null;
                            this.isThreatened = false;
                            monster.isThreatened = false;
                            monster.state = State.WANDER;
                        }
                        this.state = State.WANDER;
                        this.avoidTimer = this.avoidTime;
                    }
                    else {
                        this.panicVel -= this.panicAccel * this.panicVel.normalized * Time.deltaTime;
                    }
                }
                else if (displacement.magnitude > 0.01) {
                    this.panicVel -= displacement.normalized * this.panicAccel * Time.deltaTime;
                    if (this.panicVel.magnitude > this.panicSpeed) {
                        this.panicVel *= this.panicSpeed / this.panicVel.magnitude;
                    }
                }
                this.transform.position += (Vector3)this.panicVel * Time.deltaTime;
            }
            if (this.state == State.DEAD) {
                this.transform.position += (Vector3)this.deadVel * Time.deltaTime;
                if (this.deadVel.magnitude > 1f) {
                    this.deadVel -= this.deadVel.normalized * 4f * Time.deltaTime;
                }
                else {
                    this.deadVel = Vector2.zero;
                }
            }
        }
    }

    public virtual bool IsFirey() {
        return false;
    }

    public virtual bool CanBurn() {
        return true;
    }

    public void Enflame() {
        if (this.CanBurn()) {
            this.enflamed = true;
        }
    }

    public void Extinguish() {
        this.enflamed = false;
    }

    public bool CanPickup() {
        return this.canPickUp && (this.state == State.WANDER || this.state == State.DEAD);
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster wanders into range.
    protected virtual State ChooseThreatenOffensive(Monster other) {
        return State.WANDER;
    }

    // Choose whether to threaten, panic, or just keep wandering when another monster threatens.
    protected virtual State ChooseThreatenDefensive(Monster other) {
        return State.PANIC;
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
