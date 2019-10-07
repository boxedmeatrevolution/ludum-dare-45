using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float lifetime = 0.6f;
    private float tickTimer;
    private float explosionRadius = 1f;
    // Start is called before the first frame update
    void Start()
    {
        this.tickTimer = 0.1f;
        // Choose z position.
        Vector2 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, pos.y, -10f);
        for (uint i = 0; i < 15; ++i) {
            GameObject particleObj = Instantiate(PrefabManager.FIRE_PARTICLE_PREFAB, this.transform.position, Quaternion.identity);
            Particle particle = particleObj.GetComponent<Particle>();
            particle.target = (Vector2)this.transform.position + this.explosionRadius * Random.insideUnitCircle;
        }

        // Enflame orbs.
        foreach (Orb orb in FindObjectsOfType<Orb>()) {
            if (orb.enflamed) {
                continue;
            }
            if (orb.item.state == Item.State.BEING_DROPPED || orb.item.state == Item.State.ON_GROUND || orb.item.state == Item.State.PICKED_UP || orb.item.state == Item.State.BEING_LIFTED) {
                float distance = ((Vector2)orb.item.transform.position - (Vector2)this.transform.position).magnitude;
                if (distance < this.explosionRadius) {
                    orb.Enflame();
                }
            }
        }

        // Enflame monsters.
        foreach (Monster monster in FindObjectsOfType<Monster>()) {
            if (!monster.Initialized()) {
                continue;
            }
            float distance = ((Vector2)monster.transform.position - (Vector2)this.transform.position).magnitude;
            if (distance < this.explosionRadius) {
                monster.Enflame();
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        this.lifetime -= Time.deltaTime;
        if (this.lifetime < 0f)
        {
            Destroy(this.gameObject);
        }
        this.tickTimer -= Time.deltaTime;
        if (this.tickTimer < 0f)
        {
            this.transform.rotation *= Quaternion.Euler(0f, 0f, Random.Range(90f, 270f));
            this.tickTimer = 0.1f;
        }
    }
}
