using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static GameObject FIGHT_CLOUD_PREFAB;
    public static GameObject FIRE_PREFAB;
    public static GameObject EXPLOSION_PREFAB;

    public static GameObject COPYCAT_PREFAB;
    public static GameObject FIRE_SALAMANDER_PREFAB;
    public static GameObject GHOST_SLUG_PREFAB;
    public static GameObject GOBLIN_PREFAB;
    public static GameObject LIVING_FLAME_PREFAB;
    public static GameObject PLANT_OGRE_PREFAB;

    public GameObject fightCloudPrefab;
    public GameObject firePrefab;
    public GameObject explosionPrefab;

    public GameObject copycatPrefab;
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

    public static GameObject GetMonsterPrefab(Type type) {
        if (type.Equals(typeof(Copycat)) {
            return PrefabManager.COPYCAT_PREFAB;
        }
        else if (type.Equals(typeof(FireSalamander))) {
            return PrefabManager.FIRE_SALAMANDER_PREFAB;
        }
        else if (type.Equals(typeof(GhostSlug))) {
            return PrefabManager.GHOST_SLUG_PREFAB;
        }
        else if (type.Equals(typeof(Goblin))) {
            return PrefabManager.GOBLIN_PREFAB;
        }
        else if (type.Equals(typeof(LivingFlame))) {
            return PrefabManager.LIVING_FLAME_PREFAB;
        }
        else if (type.Equals(typeof(PlantOgre))) {
            return PrefabManager.PLANT_OGRE_PREFAB;
        }
        else {
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
