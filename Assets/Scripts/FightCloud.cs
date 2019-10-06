using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightCloud : MonoBehaviour
{
    public Monster fighter1 = null;
    public Monster fighter2 = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 averagePosition = 0.5f * (this.fighter1.transform.position + this.fighter2.transform.position);
        this.transform.position = averagePosition;
    }

    public void FightOver() {
        Destroy(this.gameObject);
    }
}
