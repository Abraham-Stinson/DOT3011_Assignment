using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightWeapon : WeaponBase
{

    [SerializeField] private LightBeamShooter narrowShooter;
    [SerializeField] private LightBeamShooter wideShooter;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    public override void MainAttack()
    {
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        narrowShooter.Shoot(origin, direction);
    }

    public override void SecondaryAttack()
    {
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        wideShooter.Shoot(origin, direction);
    }

}
