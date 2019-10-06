using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
    private readonly static float FIGHT_TIME = 2f;
    private readonly static float POST_FIGHT_TIME = 3f;
    private readonly static float DYING_TIME = 1f;
    private float stateTimer;
    private Vector2 velocity;
    private State state;
    private bool enflamed;
    private Item item;

    private Pen pen;
    private Vector2 waypoint;
    private Monster avoid;

    private Fire fire;
    private new SpriteRenderer renderer;
    private Emotion emotion;

    private FightCloud fightCloud;

    // No matter what, any pairwise interactions require one and only one partner.
    private Monster target;

    public float friction = 2f;
    public float fireHeight = 0.5f;
    public float accel = 3f;
    public float wanderSpeed = 0.5f;
    public float sprintSpeed = 2f;
    public float fleeRadius = 2.5f;
    public float threatenRadius = 1.5f;
    public float averageWaitTime = 5f;
    public float threatenTime = 2f;

    public enum State {
        WANDER,
        THREATEN,
        FLEE,
        IGNORE,
        PRE_FIGHT,
        FIGHT,
        POST_FIGHT,
        ENFLAMED_PANIC,
        LURE, // One monster lures, the other is mesmerized.
        MESMERIZED,
        GOOED, // Get gooed by slugs
        DYING,
        DEAD
    }

    // Start is called before the first frame update
    protected virtual void Start() {
        this.stateTimer = 0f;
        this.velocity = Vector2.zero;
        this.enflamed = false;
        this.pen = FindObjectOfType<Pen>();
        this.item = GetComponent<Item>();
        this.renderer = GetComponentInChildren<SpriteRenderer>();
        this.emotion = GetComponentInChildren<Emotion>();
        this.fightCloud = null;
        this.fire = null;
        this.target = null;
        this.avoid = null;

        this.waypoint = this.ChooseWaypoint(this.pen);
    }

    protected virtual void Update() {
        if (this.state == State.WANDER) {
            // Actions.
            Vector2 displacement = this.waypoint - (Vector2)this.transform.position;
            if (displacement.magnitude > 0.1) {
                // Move towards waypoint, avoiding the other monster.
                this.velocity += this.accel * displacement.normalized * Time.deltaTime;
                if (this.avoid != null) {
                    Vector2 avoidDisplacement = this.avoid.transform.position - this.transform.position;
                    if (avoidDisplacement.magnitude < this.fleeRadius) {
                        this.velocity -= this.accel * avoidDisplacement.normalized * Time.deltaTime;
                    }
                }
            }
            else {
                // Choose a new waypoint.
                if (Random.value > (1 - Time.deltaTime / this.averageWaitTime)) {
                    this.waypoint = this.ChooseWaypoint(this.pen);
                }
            }
            // Transitions.
            foreach (Monster monster in FindObjectsOfType<Monster>()) {
                if (monster == this || this.state != State.WANDER) {
                    continue;
                }
                // Can only interact with monsters in the wander state.
                if (monster.state == State.WANDER) {
                    Vector2 monsterDisplacement = monster.transform.position - this.transform.position;
                    // Can only interact with monsters smaller than threaten radius.
                    if (monsterDisplacement.magnitude < this.threatenRadius) {
                        State choice = this.ChooseThreatenOffensive(monster);
                        State monsterChoice = monster.ChooseThreatenOffensive(this);
                        bool isThreaten = false;
                        if (choice == State.FLEE) {
                            this.state = State.FLEE;
                            this.target = monster;
                        }
                        if (monsterChoice == State.FLEE) {
                            monsterChoice = State.FLEE;
                            monster.target = this;
                        }
                        if (choice == State.THREATEN) {
                            isThreaten = true;
                            this.state = State.THREATEN;
                            this.stateTimer = this.threatenTime;
                            this.target = monster;
                            monster.avoid = this;
                        }
                        if (monsterChoice == State.THREATEN) {
                            isThreaten = true;
                            monster.state = State.THREATEN;
                            monster.stateTimer = monster.threatenTime;
                            monster.target = this;
                            this.avoid = monster;
                        }
                        if (isThreaten && choice == State.IGNORE) {
                            this.state = State.IGNORE;
                            this.target = monster;
                        }
                        if (isThreaten && monsterChoice == State.IGNORE) {
                            monster.state = State.IGNORE;
                            monster.target = this;
                        }
                    }
                }
            }

        }
        else if (this.state == State.IGNORE) {
            Monster monster = this.target;
            Vector2 displacement = this.waypoint - (Vector2)this.transform.position;
            if (displacement.magnitude > 0.1) {
                // Move towards waypoint.
                this.velocity += this.accel * displacement.normalized * Time.deltaTime;
            }
            else {
                // Choose a new waypoint.
                if (Random.value > (1 - Time.deltaTime / this.averageWaitTime)) {
                    this.waypoint = this.ChooseWaypoint(this.pen);
                }
            }
        }
        else if (this.state == State.THREATEN) {
            this.stateTimer -= Time.deltaTime;
            if (this.stateTimer < 0f) {
                Monster monster = this.target;
                float distance = (monster.transform.position - this.transform.position).magnitude;
                if (monster.state != State.FLEE) {
                    this.state = State.PRE_FIGHT;
                }
                if (distance > this.fleeRadius && distance > monster.fleeRadius) {
                    this.state = State.WANDER;
                }
            }
        }
        else if (this.state == State.PRE_FIGHT) {
            Monster monster = this.target;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (displacement.magnitude > 0.1) {
                this.velocity += this.accel * displacement.normalized * Time.deltaTime;
            }
            else {
                this.state = State.FIGHT;
                this.stateTimer = Monster.FIGHT_TIME;
                monster.state = State.FIGHT;
                monster.stateTimer = Monster.FIGHT_TIME;
                if (this.IsFirey() || this.enflamed) {
                    monster.Enflame();
                }
                if (monster.IsFirey() || monster.enflamed) {
                    this.Enflame();
                }

                // Make fight cloud.
                GameObject fightCloudObj = Instantiate(PrefabManager.FIGHT_CLOUD_PREFAB, this.transform.position, Quaternion.identity);
                this.fightCloud = fightCloudObj.GetComponent<FightCloud>();
                monster.fightCloud = this.fightCloud;
                this.fightCloud.fighter1 = this;
                this.fightCloud.fighter2 = monster;
            }
        }
        else if (state == State.FIGHT) {
            Monster monster = this.target;
            this.velocity = Vector2.zero;
            this.stateTimer -= Time.deltaTime;
            if (this.stateTimer < 0f) {
                // Destroy fight cloud.
                Destroy(this.fightCloud.gameObject);
                this.fightCloud = null;
                if (this.KillOpponent(monster)) {
                    monster.state = State.DYING;
                    monster.stateTimer = Monster.DYING_TIME;
                    monster.velocity = new Vector2(
                        Random.Range(1f, 2f),
                        Random.Range(-1f, 1f));
                    monster.target = null;
                }
                else {
                    monster.state = State.POST_FIGHT;
                    monster.stateTimer = Monster.POST_FIGHT_TIME;
                }
                if (monster.KillOpponent(this)) {
                    this.state = State.DYING;
                    this.stateTimer = Monster.DYING_TIME;
                    this.velocity = new Vector2(
                        Random.Range(1f, 2f),
                        Random.Range(-1f, 1f));
                    this.target = null;
                }
                else {
                    this.state = State.POST_FIGHT;
                    this.stateTimer = Monster.POST_FIGHT_TIME;
                }
            }
        }
        else if (this.state == State.POST_FIGHT) {
            Monster monster = this.target;
            this.stateTimer -= Time.deltaTime;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (this.stateTimer < 0f && displacement.magnitude > this.fleeRadius && displacement.magnitude > monster.fleeRadius) {
                this.state = State.WANDER;
                this.waypoint = this.ChooseWaypoint(this.pen);
            }
            else {
                if (displacement.magnitude == 0f) {
                    float angle = 2 * Mathf.PI * Random.value;
                    displacement = 0.1f * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }
                this.velocity -= this.accel * displacement.normalized * Time.deltaTime;
            }
        }
        else if (this.state == State.FLEE) {
            Monster monster = this.target;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (displacement.magnitude == 0f) {
                float angle = 2 * Mathf.PI * Random.value;
                displacement = 0.1f * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
            this.velocity -= this.accel * displacement.normalized * Time.deltaTime;
            if (displacement.magnitude > this.fleeRadius && displacement.magnitude > monster.fleeRadius) {
                this.state = State.WANDER;
                if (monster.state == State.THREATEN || monster.state == State.FLEE) {
                    monster.state = State.WANDER;
                }
            }
        }
        else if (this.state == State.DYING) {
            this.stateTimer -= Time.deltaTime;
            if (this.stateTimer < 0f) {
                this.state = State.DEAD;
                if (this.IsExplosive()) {
                    Instantiate(PrefabManager.EXPLOSION_PREFAB, this.transform.position, Quaternion.identity);
                }
            }
        }

        // Physics
        float speed = this.velocity.magnitude;
        float maxSpeed = (this.state == State.WANDER || this.state == State.POST_FIGHT || this.state == State.IGNORE) ?
            this.wanderSpeed : this.sprintSpeed;
        if (speed != 0f) {
            Vector2 frictionChange = this.velocity.normalized * this.friction * Time.deltaTime;
            if (frictionChange.magnitude < speed) {
                this.velocity -= frictionChange;
            } else {
                this.velocity = Vector2.zero;
            }
        }
        if (speed > maxSpeed) {
            this.velocity *= maxSpeed / speed;
        }
        this.transform.position += (Vector3)this.velocity * Time.deltaTime;
        this.emotion.UpdateFromState(state);
    }

    // Choose whether to threaten, flee, or ignore when another monster wanders into range.
    protected virtual State ChooseThreatenOffensive(Monster other) {
        return State.WANDER;
    }

    // Choose whether to threaten, flee, or ignore when another monster threatens.
    protected virtual State ChooseThreatenDefensive(Monster other) {
        return State.FLEE;
    }

    public void Enflame() {
        if (this.CanBurn()) {
            this.enflamed = true;
            GameObject fireObj = Instantiate(PrefabManager.FIRE_PREFAB, this.renderer.transform);
            this.fire = fireObj.GetComponentInChildren<Fire>();
            this.fire.transform.localPosition = new Vector2(0, this.fireHeight - this.renderer.transform.localPosition.y);
        }
    }

    public virtual Vector2 ChooseWaypoint(Pen pen) {
        float x1 = pen.transform.position.x;
        float x2 = x1 + pen.width;
        float y1 = pen.transform.position.y;
        float y2 = y1 + pen.height;
        Vector2 result = (Vector2)this.transform.position + 256f * Random.insideUnitCircle;
        result.x = Mathf.Clamp(result.x, x1, x2);
        result.y = Mathf.Clamp(result.y, y1, y2);
        return result;
    }

    public void Extinguish() {
        this.enflamed = false;
        Destroy(this.fire.gameObject);
        this.fire = null;
    }

    public virtual bool IsFirey() {
        return false;
    }

    public virtual bool CanBurn() {
        return true;
    }

    public virtual bool IsExplosive() {
        return false;
    }

    public virtual bool KillOpponent(Monster other) {
        return false;
    }

    public bool CanPickup() {
        if (this.state == State.IGNORE) {
            Monster monster = this.target;
            if (monster.state == State.THREATEN || monster.state == State.FLEE) {
                monster.state = State.WANDER;
                monster.target = null;
            }
            return true;
        }
        return this.state == State.WANDER || this.state == State.DYING;
    }
}
