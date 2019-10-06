using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponentInChildren<Animator>();
        this.animator.Play("Explosion");
    }

    // Update is called once per frame
    public void Update()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Monster monster = collision.otherCollider.GetComponent<Monster>();
        if (monster != null) {
            monster.Enflame();
        }
    }
}
