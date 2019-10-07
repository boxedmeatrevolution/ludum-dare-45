using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour
{

    private Transform spawnWaypoint;
    private Transform frontalWaypoint;
    private Transform spawnEndPoint1;
    private Transform spawnEndPoint2;
    private Transform[] endpoints = new Transform[2];
    private int endpointCounter = 0;
    public AudioSource audioSource;
    public AudioClip voidSummonClip;
    // Start is called before the first frame update
    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        this.spawnWaypoint = GameObject.Find("SpawnWaypoint").GetComponentInChildren<Transform>();
        this.frontalWaypoint = GameObject.Find("FrontalWaypoint").GetComponentInChildren<Transform>();
        this.spawnEndPoint1 = GameObject.Find("SpawnEndPoint1").GetComponentInChildren<Transform>();
        this.spawnEndPoint2 = GameObject.Find("SpawnEndPoint2").GetComponentInChildren<Transform>();
        this.endpoints[0] = this.spawnEndPoint1;
        this.endpoints[1] = this.spawnEndPoint2;
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
                this.SendToVoid(monster);
                break;
            }
        }
    }

    public void SendToVoid(Monster monster)
    {
        monster.SendToVoid(this.spawnWaypoint.position);
    }

    public void EjectMonster(Monster monster)
    {
        this.endpointCounter++;
        monster.createdFromVoid = true;
        monster.EjectFromTo(this.spawnWaypoint.position, this.endpoints[endpointCounter % this.endpoints.Length].position);
    }
}
