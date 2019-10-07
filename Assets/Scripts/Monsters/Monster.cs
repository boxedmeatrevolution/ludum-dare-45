using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
    public readonly static float FIGHT_TIME = 4f;
    public readonly static float POST_FIGHT_TIME = 3f;
    public readonly static float DYING_TIME = 4f;
    public readonly static float DEVOUR_TIME = 1f;
    public readonly static float DIGEST_TIME = 4f;
    public readonly static float GOO_TIME = 5f;

    private bool initialized = false;
    private bool copycat;
    private float stateTimer;
    private Vector2 velocity;
    public State state;
    private bool enflamed;
    private Item item;
    // Shoved allows a monster to temporarily move faster than its max velocity.
    private bool shoved;

    private Pen pen;
    private Gate gate;
    private Vector2 waypoint;
    private Monster avoid;

    private Fire fire;
    private new SpriteRenderer renderer;
    private Emotion emotion;

    private FightCloud fightCloud;

    // No matter what, any pairwise interactions require one and only one partner.
    private Monster target;

    public GameObject fireAnchor;
    public float friction = 1f;
    public float accel = 3f;
    public float wanderSpeed = 0.5f;
    public float sprintSpeed = 2f;
    public float fleeRadius = 2.5f;
    public float threatenRadius = 1.5f;
    public float averageWaitTime = 5f;
    public float threatenTime = 2f;

    private Vector3 voidWaypoint;
    private Vector3 voidStartpoint;
    private float voidWaypointVel = 0f;
    private float voidWaypointAccel = 0.0005f;

    private float deadTime = 0f;
    private float deadWaitTime = 1f;
    private Void voido;

    private float forgetAvoidTimer = 0f;

    public Orb transformOrb;
    private float transformOrbStartTime = 0f;
    private float transformOrbTotalTime = 1f;

    public bool createdFromVoid = false;

    public Orb[] orbs = new Orb[0];
    private OrbManager orbManager;

    private float ambientSoundTimer = 2f;
    private float fightNoiseTimer;

    private AudioSource audioSource;
    public AudioClip threatenSound;
    public AudioClip fleeSound;
    public AudioClip fightSound;
    public AudioClip footstepSound;
    public AudioClip ambient1Sound;
    public AudioClip ambient2Sound;
    public AudioClip ambient3Sound;
    public AudioClip lureSound;
    public AudioClip deathSound;
    public AudioClip spawnSound;


    private Main main;

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
        DEVOURING,
        BEING_DEVOURED,
        DIGESTING,
        GOOED, // Get gooed by slugs
        DYING,
        DEAD,
        SENDING_TO_VOID,
        IN_VOID,
        SPAWNING_FROM_VOID,
        TRAVEL_TO_TRANSFORM_ORB,
        TRANSFORM_ORB,
        ORBED
    }

    // Start is called before the first frame update
    protected virtual void Start() {
        this.initialized = true;
        this.copycat = false;
        this.stateTimer = 0f;
        this.velocity = Vector2.zero;
        this.enflamed = false;
        this.pen = FindObjectOfType<Pen>();
        this.gate = FindObjectOfType<Gate>();
        this.item = GetComponent<Item>();
        this.renderer = GetComponentInChildren<SpriteRenderer>();
        this.emotion = GetComponentInChildren<Emotion>();
        this.shoved = false;
        this.fightCloud = null;
        this.fire = null;
        this.target = null;
        this.avoid = null;
        this.state = State.WANDER;
        this.waypoint = this.ChooseWaypoint(this.pen);
        this.orbManager = GameObject.Find("OrbManager").GetComponent<OrbManager>();
        this.main = GameObject.Find("Main").GetComponent<Main>();
        this.audioSource = GetComponent<AudioSource>();


        if (this.createdFromVoid)
        {
            this.state = State.SPAWNING_FROM_VOID;
            this.transform.localScale = new Vector3(0f, 0f, 1f);
        }

        this.voido = GameObject.Find("Void").GetComponent<Void>();

    }

    protected virtual void Update() {
        if (this.state == State.IN_VOID) {
            Destroy(this.gameObject);
        }
        if (this.state == State.SENDING_TO_VOID)
        {
            this.MoveToVoidWaypoint(true);
            if (this.IsAtVoidWaypoint())
            {
                this.state = State.IN_VOID;
                this.audioSource.PlayOneShot(this.spawnSound);
            }
            return;
        }
        if (this.state == State.IN_VOID)
        {
            this.ReturnOrbs();
            this.renderer.enabled = false;
        }
        if (this.state == State.SPAWNING_FROM_VOID)
        {
            this.MoveToVoidWaypoint(false);
            if (this.IsAtVoidWaypoint())
            {
                this.state = State.WANDER;
                this.item.state = Item.State.ON_GROUND;
                this.transform.localScale = new Vector3(1f, 1f, 1f);
                this.audioSource.PlayOneShot(this.spawnSound);
            }
            return;
        }

        if (this.item.state == Item.State.PICKED_UP && !this.ShouldBeDropped())
        {
            this.main.DropItem();
            return;
        } 

        if (this.item.state != Item.State.ON_GROUND) {
            return;
        }
        if (this.state == State.WANDER) {
            // Sound.
            this.ambientSoundTimer -= Time.deltaTime;
            if (this.ambientSoundTimer < 0f) {
                this.ambientSoundTimer = Random.Range(8f, 14f);
                this.PlayAmbientSound();
            }
            // Actions.
            Vector2 displacement = this.waypoint - (Vector2)this.transform.position;
            if (displacement.magnitude > 0.1) {
                // Move towards waypoint, avoiding the other monster.
                this.Accel(this.accel * displacement.normalized);
                if (this.avoid != null) {
                    Vector2 avoidDisplacement = this.avoid.transform.position - this.transform.position;
                    if (avoidDisplacement.magnitude < this.fleeRadius) {
                        this.Accel(-this.accel * avoidDisplacement.normalized);
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
                if (monster == this || this.state != State.WANDER || !monster.Initialized() || monster.state != State.WANDER || monster.item.state != Item.State.ON_GROUND) {
                    continue;
                }
                Vector2 monsterDisplacement = monster.transform.position - this.transform.position;
                // Can only interact with monsters smaller than threaten radius.
                if (monsterDisplacement.magnitude < this.threatenRadius) {
                    State choice = this.ChooseThreatenOffensive(monster);
                    State monsterChoice = monster.ChooseThreatenOffensive(this);
                    if (choice == State.THREATEN || choice == State.LURE) {
                        monsterChoice = monster.ChooseThreatenDefensive(this);
                    }
                    if (monsterChoice == State.THREATEN || monsterChoice == State.LURE) {
                        choice = this.ChooseThreatenDefensive(monster);
                    }
                    bool isThreaten = (choice == State.THREATEN || monsterChoice == State.THREATEN);
                    bool isLure = (choice == State.LURE || monsterChoice == State.LURE);
                    if (!isLure) {
                        if (choice == State.FLEE) {
                            this.state = State.FLEE;
                            this.target = monster;
                            this.audioSource.Stop();
                            this.audioSource.PlayOneShot(this.fleeSound, 0.75f);
                        }
                        if (monsterChoice == State.FLEE) {
                            monster.state = State.FLEE;
                            monster.target = this;
                            monster.audioSource.Stop();
                            monster.audioSource.PlayOneShot(monster.fleeSound, 0.75f);
                        }
                        if (choice == State.THREATEN) {
                            this.state = State.THREATEN;
                            this.stateTimer = this.threatenTime;
                            this.target = monster;
                            monster.Avoid(this);
                            this.Avoid(monster);
                            this.audioSource.Stop();
                            this.audioSource.PlayOneShot(this.threatenSound, 0.75f);
                        }
                        if (monsterChoice == State.THREATEN) {
                            monster.state = State.THREATEN;
                            monster.stateTimer = monster.threatenTime;
                            monster.target = this;
                            this.Avoid(monster);
                            monster.Avoid(this);
                            monster.audioSource.Stop();
                            monster.audioSource.PlayOneShot(monster.threatenSound, 0.75f);
                        }
                        if (isThreaten) {
                            if (choice == State.IGNORE) {
                                this.state = State.IGNORE;
                                this.target = monster;
                            }
                            if (monsterChoice == State.IGNORE) {
                                monster.state = State.IGNORE;
                                monster.target = this;
                            }
                        }
                    } else {
                        if (choice == State.LURE) {
                            this.state = State.LURE;
                            monster.state = State.MESMERIZED;
                            this.target = monster;
                            monster.target = this;
                            this.audioSource.Stop();
                            monster.audioSource.Stop();
                            this.audioSource.PlayOneShot(this.lureSound, 0.75f);
                        }
                        if (monsterChoice == State.LURE) {
                            this.state = State.MESMERIZED;
                            monster.state = State.LURE;
                            this.target = monster;
                            monster.target = this;
                            this.audioSource.Stop();
                            monster.audioSource.Stop();
                            monster.audioSource.PlayOneShot(monster.lureSound, 0.75f);
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
                this.Accel(this.accel * displacement.normalized);
            }
            else {
                // Choose a new waypoint.
                if (Random.value > (1 - Time.deltaTime / this.averageWaitTime)) {
                    this.waypoint = this.ChooseWaypoint(this.pen);
                }
            }
        }
        else if (this.state == State.THREATEN) {
            if (this.stateTimer < 0f) {
                Monster monster = this.target;
                float distance = ((Vector2)monster.transform.position - (Vector2)this.transform.position).magnitude;
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
                this.Accel(this.accel * displacement.normalized);
            }
            else {
                this.state = State.FIGHT;
                this.stateTimer = Monster.FIGHT_TIME;
                monster.state = State.FIGHT;
                monster.stateTimer = Monster.FIGHT_TIME;

                // Make fight cloud.
                GameObject fightCloudObj = Instantiate(PrefabManager.FIGHT_CLOUD_PREFAB, this.transform.position, Quaternion.identity);
                this.fightCloud = fightCloudObj.GetComponent<FightCloud>();
                monster.fightCloud = this.fightCloud;
                this.fightCloud.fighter1 = this;
                this.fightCloud.fighter2 = monster;
                this.fightNoiseTimer = 0.2f;
                monster.fightNoiseTimer = float.PositiveInfinity;
            }
        }
        else if (state == State.FIGHT) {
            this.fightNoiseTimer -= Time.deltaTime;
            if (this.fightNoiseTimer < 0f) {
                this.fightNoiseTimer = Random.Range(0.3f, 0.5f);
                this.audioSource.pitch = Random.Range(0.9f, 1.1f);
                this.audioSource.PlayOneShot(this.fightSound, 0.3f);
            }
            Monster monster = this.target;
            this.velocity = Vector2.zero;
            if (Random.value > 1 - Time.deltaTime / 0.2f) {
                this.transform.rotation *= Quaternion.Euler(0f, 0f, Random.Range(0, 360f));
            }
            if (this.stateTimer < 0f) {
                this.transform.rotation = Quaternion.identity;
                monster.transform.rotation = Quaternion.identity;
                // Destroy fight cloud.
                Destroy(this.fightCloud.gameObject);
                this.fightCloud = null;
                // Give velocities.
                monster.Shove(new Vector2(
                    Random.Range(0.4f, 0.6f),
                    Random.Range(-0.1f, 0.1f)));
                this.Shove(new Vector2(
                    Random.Range(-0.6f, -0.4f),
                    Random.Range(-0.1f, 0.1f)));
                // Burn
                if (this.IsFiery()) {
                    monster.Enflame();
                }
                if (monster.IsFiery()) {
                    this.Enflame();
                }
                if (this.KillOpponent(monster)) {
                    monster.state = State.DYING;
                    monster.stateTimer = Monster.DYING_TIME;
                    monster.OnDying();
                    monster.target = null;
                    monster.audioSource.pitch = 1f;
                    monster.audioSource.Stop();
                    monster.audioSource.PlayOneShot(monster.deathSound, 0.6f);
                }
                else {
                    monster.state = State.POST_FIGHT;
                    monster.stateTimer = Monster.POST_FIGHT_TIME;
                    monster.audioSource.pitch = 1f;
                }
                if (monster.KillOpponent(this)) {
                    this.state = State.DYING;
                    this.stateTimer = Monster.DYING_TIME;
                    this.OnDying();
                    this.target = null;
                    this.audioSource.pitch = 1f;
                    this.audioSource.Stop();
                    this.audioSource.PlayOneShot(this.deathSound, 0.6f);
                }
                else {
                    this.state = State.POST_FIGHT;
                    this.stateTimer = Monster.POST_FIGHT_TIME;
                    this.audioSource.pitch = 1f;
                }
            }
        }
        else if (this.state == State.POST_FIGHT) {
            Monster monster = this.target;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (this.stateTimer < 0f) {
                bool farEnough = (displacement.magnitude > this.fleeRadius && displacement.magnitude > monster.fleeRadius);
                bool monsterDead = (monster.state == State.DEAD || monster.state == State.DYING);
                if (farEnough || monsterDead) {
                    this.state = State.WANDER;
                    this.waypoint = this.ChooseWaypoint(this.pen);
                }
            }
            else {
                if (displacement.magnitude == 0f) {
                    float angle = 2 * Mathf.PI * Random.value;
                    displacement = 0.1f * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }
                this.Accel(-this.accel * displacement.normalized);
            }
        }
        else if (this.state == State.LURE) {
            Monster monster = this.target;
            float distance = ((Vector2)monster.transform.position - (Vector2)this.transform.position).magnitude;
            if (distance < 0.5f) {
                this.state = State.DEVOURING;
                monster.state = State.BEING_DEVOURED;
                this.stateTimer = Monster.DEVOUR_TIME;
                monster.stateTimer = Monster.DEVOUR_TIME;
            }
        }
        else if (this.state == State.MESMERIZED) {
            Monster monster = this.target;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (displacement.magnitude == 0f) {
                float angle = 2 * Mathf.PI * Random.value;
                displacement = 0.1f * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
            this.Accel(this.accel * displacement.normalized);
        }
        else if (this.state == State.DEVOURING) {
            Monster monster = this.target;
            if (this.stateTimer < 0f) {
                this.state = State.DIGESTING;
                this.stateTimer = Monster.DIGEST_TIME;
                monster.gameObject.SetActive(false);
            }
        }
        else if (this.state == State.BEING_DEVOURED) {
            Monster monster = this.target;
            float scale = this.stateTimer / Monster.DEVOUR_TIME;
            Vector2 mouthPosition = monster.transform.position;
            Vector2 displacement = mouthPosition - (Vector2)this.transform.position;
            if (displacement.magnitude == 0f) {
                float angle = 2 * Mathf.PI * Random.value;
                displacement = 0.1f * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
            this.velocity = Vector2.zero;
            this.transform.position = mouthPosition - displacement * scale;
            this.transform.localScale = new Vector3((scale + 0.2f) / 1.2f, (scale + 0.2f) / 1.2f, 1f);
        }
        else if (this.state == State.DIGESTING) {
            Monster monster = this.target;
            if (this.stateTimer < 0f) {
                this.state = State.WANDER;
                this.target = null;
                monster.target = null;
                monster.gameObject.SetActive(true);
                monster.transform.localScale = new Vector3(1f, 1f, 1f);
                if (this.KillDigestedOpponent(monster)) {
                    Destroy(monster.gameObject);
                } else {
                    monster.state = State.GOOED;
                    monster.stateTimer = Monster.GOO_TIME;
                    monster.Extinguish();
                    if (monster.IsFireElemental()) {
                        monster.OnDying();
                    }
                    GameObject ectoplasmObj = Instantiate(PrefabManager.ECTOPLASM_PREFAB, monster.transform);
                    ectoplasmObj.transform.localPosition = new Vector3(0f, -0.01f, 0f);
                    float angle = 2f * Mathf.PI * Random.value;
                    monster.Shove(Random.Range(0.2f, 0.3f) * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
                }
            }
        }
        else if (this.state == State.GOOED) {
            if (this.stateTimer < 0f) {
                if (this.IsFireElemental()) {
                    this.state = State.DEAD;
                }
                else {
                    this.state = State.WANDER;
                }
            }
        }
        else if (this.state == State.FLEE) {
            Monster monster = this.target;
            Vector2 displacement = monster.transform.position - this.transform.position;
            if (displacement.magnitude == 0f) {
                float angle = 2 * Mathf.PI * Random.value;
                displacement = 0.1f * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
            this.Accel(-this.accel * displacement.normalized);
            if (displacement.magnitude > this.fleeRadius && displacement.magnitude > monster.fleeRadius) {
                this.state = State.WANDER;
                if (monster.state == State.THREATEN || monster.state == State.FLEE) {
                    monster.state = State.WANDER;
                }
            }
        }
        else if (this.state == State.DYING) {
            if (this.stateTimer < 0f) {
                this.state = State.DEAD;
                this.deadTime = Time.realtimeSinceStartup;
                if (this.IsExplosive()) {
                    Instantiate(PrefabManager.EXPLOSION_PREFAB, this.transform.position, Quaternion.identity);
                }
            }
        }
        else if (this.state == State.DEAD)
        {
            if (Time.realtimeSinceStartup - this.deadTime > this.deadWaitTime)
            {
                if (this.voido != null)
                {
                    this.voido.SendToVoid(this);
                }
            }
        }
        else if (this.state == State.TRAVEL_TO_TRANSFORM_ORB)
        {
            if (this.transformOrb == null)
            {
                this.state = State.WANDER;
            }
            else
            {
                Vector2 displacement = this.transformOrb.transform.position - this.transform.position;
                if (displacement.magnitude > 0.5f) {
                    this.Accel(this.accel * displacement.normalized);
                }
                else
                {
                    this.state = State.TRANSFORM_ORB;
                    this.transformOrbStartTime = Time.realtimeSinceStartup;
                }

            }
        }
        else if (this.state == State.TRANSFORM_ORB)
        {
            float delta = Time.realtimeSinceStartup - this.transformOrbStartTime;
            if (delta < this.transformOrbTotalTime)
            {
                float scale = Mathf.Abs((delta / this.transformOrbTotalTime) - 1);
                this.transform.localScale = new Vector3(scale, scale, 1f);
            }
            else
            {
                // time to transform orb
                this.transformOrb.Extinguish();
                this.orbManager.MakeGhostOrb(this.transformOrb, this.orbs);
                this.orbs = new Orb[2];
                this.state = State.ORBED;
            }
        }
        else if (this.state == State.ORBED)
        {
            Destroy(this.gameObject);
        }
        else if (this.state == State.IN_VOID) {
            Destroy(this.gameObject);
        }

        this.forgetAvoidTimer -= Time.deltaTime;
        if (this.forgetAvoidTimer < 0f) {
            this.avoid = null;
        }

        this.stateTimer -= Time.deltaTime;
        // Physics
        float gateX = this.gate.transform.position.x + this.gate.width;
        float speed = this.velocity.magnitude;
        float maxSpeed = (this.state == State.WANDER || this.state == State.POST_FIGHT || this.state == State.IGNORE || this.state == State.MESMERIZED || this.state == State.TRAVEL_TO_TRANSFORM_ORB) ?
            this.wanderSpeed : this.sprintSpeed;
        if (speed != 0f) {
            Vector2 frictionChange = this.velocity.normalized * this.friction * Time.deltaTime;
            if (frictionChange.magnitude < speed) {
                this.velocity -= frictionChange;
            } else {
                this.velocity = Vector2.zero;
            }
        }
        speed = this.velocity.magnitude;
        if (speed > maxSpeed && !this.shoved) {
            this.velocity *= maxSpeed / speed;
        }
        if (speed < maxSpeed) {
            this.shoved = false;
        }
        if (this.transform.position.x < gateX && this.velocity.x < 0) {
            this.velocity = new Vector2(-0.5f * this.velocity.x, this.velocity.y);
        }
        this.transform.position += (Vector3)this.velocity * Time.deltaTime;
        this.emotion.UpdateFromState(state);

        // Maintain z ordering.
        Vector2 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, pos.y, pos.y / 300f);
    }

    private void PlayAmbientSound() {
        AudioClip ambientSound = null;
        if (Random.value < 0.333) {
            ambientSound = this.ambient1Sound;
        }
        else if (Random.value < 0.5) {
            ambientSound = this.ambient2Sound;
        }
        else {
            ambientSound = this.ambient3Sound;
        }
        this.audioSource.PlayOneShot(ambientSound, 0.65f);
    }

    public void Avoid(Monster monster) {
        this.avoid = monster;
        this.forgetAvoidTimer = 8f;
    }

    // Choose whether to threaten, hypnotize, flee, or ignore when another monster wanders into range.
    protected virtual State ChooseThreatenOffensive(Monster other) {
        return State.WANDER;
    }

    // Choose whether to threaten, hypnotize, flee, or ignore when another monster threatens.
    protected virtual State ChooseThreatenDefensive(Monster other) {
        return State.FLEE;
    }

    public void SetCopycat(bool copycat) {
        this.copycat = copycat;
    }

    public bool IsCopycat() {
        return this.copycat;
    }

    public bool Initialized() {
        return this.initialized;
    }

    public void Enflame() {
        if (this.CanBurn() && !this.enflamed) {
            this.enflamed = true;
            GameObject fireObj = Instantiate(PrefabManager.FIRE_PREFAB, this.fireAnchor.transform);
            this.fire = fireObj.GetComponentInChildren<Fire>();
        }
    }

    public virtual Vector2 ChooseWaypoint(Pen pen) {
        float x1 = pen.transform.position.x;
        float x2 = x1 + pen.width;
        float y1 = pen.transform.position.y;
        float y2 = y1 + pen.height;
        Vector2 result = (Vector2)this.transform.position + 0.75f * Random.insideUnitCircle;
        result.x = Mathf.Clamp(result.x, x1, x2);
        result.y = Mathf.Clamp(result.y, y1, y2);
        return result;
    }

    public bool IsPositionInPen(Vector2 pos)
    {
        float x1 = pen.transform.position.x;
        float x2 = x1 + pen.width;
        float y1 = pen.transform.position.y;
        float y2 = y1 + pen.height;

        return (pos.x > x1 && pos.x < x2 & pos.y > y1 && pos.y < y2);
    }    
    
    public bool ShouldBeDropped()
    {
        float x1 = pen.transform.position.x;
        float x2 = x1 + pen.width;
        float y1 = pen.transform.position.y;
        float y2 = y1 + pen.height;

        Vector2 pos = this.transform.position;
        return (pos.x > x1 && pos.x < x2 & pos.y > y1); // do not check upper y bound because then sometimes monster is dropped when returning to void
    }

    public void Extinguish() {
        if (this.enflamed) {
            this.enflamed = false;
            Destroy(this.fire.gameObject);
            this.fire = null;
        }
    }

    public void Accel(Vector2 accel) {
        if (!this.shoved) {
            this.velocity += accel * Time.deltaTime;
        }
    }
    public void Shove(Vector2 impulse) {
        this.velocity += impulse;
        this.shoved = true;
    }

    public virtual void OnDying() {

    }

    public bool IsFiery() {
        return this.state != State.GOOED && this.state != State.DEAD && this.state != State.DYING && (this.IsFireElemental() || this.enflamed);
    }

    public virtual bool IsFireElemental() {
        return false;
    }

    public virtual bool CanBurn() {
        return true;
    }

    public virtual bool IsExplosive() {
        return false;
    }

    public virtual bool KillDigestedOpponent(Monster other) {
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
        return this.state == State.WANDER;
    }

    public void GiveOrbs(Orb[] orbs)
    {
        this.orbs = orbs;
    }

    public Item GetItem()
    {
        return this.item;
    }

    public void SendToVoid(Vector3 waypoint)
    {
        State originalState = this.state;
        this.state = State.SENDING_TO_VOID;
        this.item.SendToVoid();
        this.voidWaypoint = waypoint;
        this.voidStartpoint = this.transform.position;
        if (originalState != State.DEAD && originalState != State.DYING) {
            this.audioSource.PlayOneShot(this.deathSound);
        }
    }

    private void MoveToVoidWaypoint(bool shrink)
    {
        if (this.voidWaypoint == null)
        {
            Debug.Log("Cannot move to waypoint: waypoint is null");
            return;
        }
        Vector2 displacement = this.voidWaypoint - this.transform.position;
        Vector2 direction = displacement.normalized;
        this.voidWaypointVel += this.voidWaypointAccel;
        Vector2 delta = direction * voidWaypointVel;
        if (delta.magnitude > displacement.magnitude)
        {
            // move the rest of the way
            this.transform.position += (Vector3)displacement;
        }
        else
        {
            // move a bit closer
            this.transform.position += (Vector3)delta;
        }

        float scale = displacement.magnitude / ((Vector2)this.voidStartpoint - (Vector2)this.voidWaypoint).magnitude;
        if (!float.IsNaN(scale))
        {
            if (!shrink)
            {
                scale = Mathf.Abs(scale - 1);
            }
            this.transform.localScale = new Vector3(scale, scale, 1f);
        }

        if (this.IsAtVoidWaypoint())
        {
            this.voidWaypointVel = 0f;
        }
    }

    private bool IsAtVoidWaypoint()
    {
        if (this.voidWaypoint == null)
        {
            Debug.Log("Cannot move to waypoint: waypoint is null");
            return true;
        }
        return ((Vector2)this.voidWaypoint - (Vector2)this.transform.position).magnitude < 0.0001f;
    }

    private void ReturnOrbs()
    {
        for (int i = 0; i < this.orbs.Length; i++)
        {
            this.orbs[i].item.ReturnToLab();
        }
        this.orbs = new Orb[0];
    }

    public void EjectFromTo(Vector3 from, Vector3 to)
    {
        this.state = State.SPAWNING_FROM_VOID;
        this.voidWaypoint = to;
        this.transform.position = from;
        this.voidStartpoint = from;
    }
}
