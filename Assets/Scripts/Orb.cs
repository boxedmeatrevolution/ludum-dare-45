using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{

    public OrbColor orbColor;
    private SpriteRenderer spriteRenderer;
    public Item item;

    public enum OrbColor
    {
        BLUE,
        RED,
        BROWN
    }

    // Start is called before the first frame update
    void Start()
    {
        this.item = GetComponent<Item>();
        this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        switch (this.orbColor)
        {
            case OrbColor.BLUE:
                this.spriteRenderer.sprite = Resources.Load<Sprite>("BlueOrb");
                break;
            case OrbColor.RED:
                this.spriteRenderer.sprite = Resources.Load<Sprite>("RedOrb");
                break;
            case OrbColor.BROWN:
                this.spriteRenderer.sprite = Resources.Load<Sprite>("BrownOrb");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.item.state == Item.State.TRANSFORMED)
        {
            this.spriteRenderer.enabled = false;
        }
        else 
        {
            this.spriteRenderer.enabled = true;
        }
    }
}
