using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbManager : MonoBehaviour
{
    private Orb[] orbs;
    private bool orbIsPickedUp;
    private Orb pickedUpOrb;

    private List<Orb> orbsInMachine;
    private List<Orb> orbsInMonster;

    // Start is called before the first frame update
    void Start()
    {
        this.orbIsPickedUp = false;
        this.orbs = GetComponentsInChildren<Orb>();
    }

    // Update is called once per frame
    void Update()
    {
        // determine if any orb is picked up
        this.orbIsPickedUp = false;
        for (int i = 0; i < this.orbs.Length; i++)
        {
            Orb orb = this.orbs[i];
            if (orb.item.pickedUp)
            {
                this.orbIsPickedUp = true;
                this.pickedUpOrb = orb;
                break;
            }
        }
    }

    public bool IsOrbPickedUp()
    {
        return this.orbIsPickedUp;
    }

    public Orb GetPickedUpOrb()
    {
        return this.pickedUpOrb;
    }
}
