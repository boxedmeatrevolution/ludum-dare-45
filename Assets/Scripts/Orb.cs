using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{

    public OrbColor orbColor;
    private SpriteRenderer spriteRenderer;
    public Item item;

    private Fire fire;
    public bool enflamed = false;
    private Transform fireAnchor;

    public List<Orb> childOrbs = new List<Orb>();

    public enum OrbColor
    {
        BLUE,
        RED,
        BROWN,
        WHITE
    }

    // Start is called before the first frame update
    void Start()
    {
        this.enflamed = false;
        this.item = GetComponent<Item>();
        Transform[] transforms = GetComponentsInChildren<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name == "FireAnchor")
            {
                this.fireAnchor = transforms[i];
            }
        }

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
            case OrbColor.WHITE:
                this.spriteRenderer.sprite = Resources.Load<Sprite>("PurpleOrb");
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

        if (this.item.state == Item.State.BEING_PUT_IN_MACHINE)
        {
            this.Extinguish();
        }

        if (this.item.state == Item.State.BEING_DROPPED)
        {
            this.BreakOrb();
        }
    }

    public void Enflame()
    {
        this.enflamed = true;
        GameObject fireObj = Instantiate(PrefabManager.FIRE_PREFAB, this.fireAnchor.transform);
        this.fire = fireObj.GetComponentInChildren<Fire>();
    }

    public void Extinguish()
    {
        this.enflamed = false;
        if (this.fire != null)
        {
            Destroy(this.fire.gameObject);
        }
    }

    public void AddChildOrb(Orb orb)
    {
        this.childOrbs.Add(orb);
    }
    
    public void BreakOrb()
    {
        if (this.orbColor == Orb.OrbColor.WHITE)
        {
            // undo the pickupz but keep the ball in place
            Vector3 removePickupZ = new Vector3(0, this.item.pickupZ, 0);
            this.item.transform.position += removePickupZ;
            for (int i = 0; i < this.childOrbs.Count; i++)
            {
                Orb orb = this.childOrbs[i];
                orb.item.transform.position = this.item.transform.position;
                orb.item.ReturnToLab();
            }
            this.childOrbs.Clear();
            this.transform.position = new Vector3(1000, 1000, 0);
        }
    }
}
