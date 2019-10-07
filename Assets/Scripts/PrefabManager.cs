using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static GameObject FIGHT_CLOUD_PREFAB;
    public static GameObject FIRE_PREFAB;
    public static GameObject EXPLOSION_PREFAB;

    public static GameObject FIRE_SALAMANDER_PREFAB;
    public static GameObject GHOST_SLUG_PREFAB;
    public static GameObject GOBLIN_PREFAB;
    public static GameObject LIVING_FLAME_PREFAB;
    public static GameObject PLANT_OGRE_PREFAB;

    public GameObject fightCloudPrefab;
    public GameObject firePrefab;
    public GameObject explosionPrefab;

    public GameObject fireSalamanderPrefab;
    public GameObject ghostSlugPrefab;
    public GameObject goblinPrefab;
    public GameObject livingFlamePrefab;
    public GameObject plantOgrePrefab;
    // Start is called before the first frame update
    void Start()
    {
        PrefabManager.FIGHT_CLOUD_PREFAB = this.fightCloudPrefab;
        PrefabManager.FIRE_PREFAB = this.firePrefab;
        PrefabManager.EXPLOSION_PREFAB = this.explosionPrefab;

        PrefabManager.FIRE_SALAMANDER_PREFAB = this.fireSalamanderPrefab;
        PrefabManager.GHOST_SLUG_PREFAB = this.ghostSlugPrefab;
        PrefabManager.GOBLIN_PREFAB = this.goblinPrefab;
        PrefabManager.LIVING_FLAME_PREFAB = this.livingFlamePrefab;
        PrefabManager.PLANT_OGRE_PREFAB = this.plantOgrePrefab;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
