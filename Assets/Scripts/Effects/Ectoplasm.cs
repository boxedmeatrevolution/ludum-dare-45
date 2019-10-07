using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ectoplasm : MonoBehaviour
{
    private float alpha0;
    private new SpriteRenderer renderer;
    private float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        this.lifetime = Monster.GOO_TIME;
        this.renderer = GetComponentInChildren<SpriteRenderer>();
        this.alpha0 = this.renderer.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        this.lifetime -= Time.deltaTime;
        if (this.lifetime < 0f) {
            Destroy(this.gameObject);
        }
        this.renderer.color = new Color(1f, 1f, 1f, (this.alpha0 - 0.1f) * (this.lifetime / Monster.GOO_TIME) + 0.1f);
    }
}
