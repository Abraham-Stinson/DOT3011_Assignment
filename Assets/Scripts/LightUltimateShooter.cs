using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUltimateShooter : MonoBehaviour, IBeam
{
    [SerializeField] private LightBeamUltimate beamUltimatePrefab;
    [SerializeField] private Transform firePoint;

    private bool isOnCooldown = false;
    private bool isActive = false;

    LightBeamUltimate spawnedLaser;

    public void Shoot(Vector3 origin, Vector3 direction)
    {
        spawnedLaser = Instantiate(beamUltimatePrefab, Vector3.zero, Quaternion.identity);
        spawnedLaser.InitiateLaser(firePoint);
    }

    private void Awake()
    {
        
    }
    
}
