using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeamShooter : MonoBehaviour, IBeam
{
    [SerializeField] private LightBeam beamPrefab;
    [SerializeField] private Transform firePoint;
    private float lastFireTime;
    private float cooldown;


    private void Awake()
    {
        if (beamPrefab.beamType == LightBeam.BeamType.Narrow)
        {
            cooldown = WeaponStatsManager.Instance.flashlightStatsRuntime.narrowCooldown;
        }
        else
        {
            cooldown = WeaponStatsManager.Instance.flashlightStatsRuntime.wideCooldown;
        }
    }
    public void Shoot(Vector3 origin, Vector3 direction)
    {
        if (Time.time - lastFireTime < cooldown) return;

        lastFireTime = Time.time;

        LightBeam beam = Instantiate(beamPrefab, firePoint.position, Quaternion.LookRotation(direction));
        beam.InitializeDirection(Camera.main);
    }
}
