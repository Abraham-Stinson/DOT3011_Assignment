using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeamShooter : MonoBehaviour, IBeam
{
    [SerializeField] private LightBeam beamPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    private float lastFireTime;

    public void Shoot(Vector3 origin, Vector3 direction)
    {
        if (Time.time - lastFireTime < fireRate) return;

        lastFireTime = Time.time;

        LightBeam beam = Instantiate(beamPrefab, firePoint.position, Quaternion.LookRotation(direction));
    }
}
