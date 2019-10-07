using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour
{

    private Transform spawnWaypoint;
    private Transform frontalWaypoint;
    private Transform spawnEndPoint1;
    private Transform spawnEndPoint2;
    // Start is called before the first frame update
    void Start()
    {
        this.spawnWaypoint = GameObject.Find("SpawnWaypoint").GetComponentInChildren<Transform>();
        this.frontalWaypoint = GameObject.Find("FrontalWaypoint").GetComponentInChildren<Transform>();
        this.spawnEndPoint1 = GameObject.Find("SpawnEndPoint1").GetComponentInChildren<Transform>();
        this.spawnEndPoint2 = GameObject.Find("SpawnEndPoint2").GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetFrontalWaypoint()
    {
        return this.frontalWaypoint.position;
    }

    public void Interact()
    {
        foreach (Monster monster in FindObjectsOfType<Monster>())
        {
            if (monster.GetItem().pickedUp)
            {
                this.ConsumeMonster(monster);
                break;
            }
        }
    }

    private void ConsumeMonster(Monster monster)
    {
        monster.SendToVoid(this.spawnWaypoint.position);
    }
}
