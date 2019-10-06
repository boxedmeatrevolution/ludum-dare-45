using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emotion : MonoBehaviour
{
    public Sprite wanderSprite;
    public Sprite threatenSprite;
    public Sprite fleeSprite;
    public Sprite fightSprite;
    public Sprite postFightSprite;
    public Sprite deadSprite;

    private new SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        this.renderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateFromState(Monster.State state) {
        Sprite sprite = null;
        if (state == Monster.State.WANDER) {
            sprite = this.wanderSprite;
        } else if (state == Monster.State.THREATEN) {
            sprite = this.threatenSprite;
        } else if (state == Monster.State.FLEE) {
            sprite = this.fleeSprite;
        } else if (state == Monster.State.PRE_FIGHT) {
            sprite = this.fightSprite;
        } else if (state == Monster.State.POST_FIGHT) {
            sprite = this.postFightSprite;
        } else if (state == Monster.State.DYING) {
            sprite = this.deadSprite;
        }
        this.renderer.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
