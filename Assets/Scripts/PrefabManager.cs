using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static GameObject FIGHT_CLOUD_PREFAB;

    public GameObject fightCloudPrefab;
    // Start is called before the first frame update
    void Start()
    {
        PrefabManager.FIGHT_CLOUD_PREFAB = this.fightCloudPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
