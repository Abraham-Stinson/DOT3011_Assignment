using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class BossTriggerManager : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPosition;
    [SerializeField, Tooltip ("Kullandıktan sonra trigger yok olacak mı?")] private bool afterUseDestroy = false;

    private bool bossHasSpawned = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !bossHasSpawned)
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        bossHasSpawned=true;
        Debug.Log("Boss Spawn Edildi!");
        Instantiate(bossPrefab, bossSpawnPosition.position, bossSpawnPosition.rotation);
        //Instantiate(spawnParticles, spawnPoint.position, Quaternion.identity); Maybe we will use

        if (afterUseDestroy)
        {
            Destroy(gameObject);
        }
    }
}
