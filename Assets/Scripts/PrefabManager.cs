using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static GameObject FIGHT_CLOUD_PREFAB;
    public static GameObject FIRE_PREFAB;
    public static GameObject EXPLOSION_PREFAB;

    public GameObject fightCloudPrefab;
    public GameObject firePrefab;
    public GameObject explosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        PrefabManager.FIGHT_CLOUD_PREFAB = this.fightCloudPrefab;
        PrefabManager.FIRE_PREFAB = this.firePrefab;
        PrefabManager.EXPLOSION_PREFAB = this.explosionPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
