using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightWeapon : WeaponBase
{

    [SerializeField] private LightBeamShooter narrowShooter;
    [SerializeField] private LightBeamShooter wideShooter;
    private bool isNarrow=true;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    public override void Fire()
    {
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        if (isNarrow)
            narrowShooter.Shoot(origin, direction);
        else
            wideShooter.Shoot(origin, direction);
    }

    public override void SwitchStance()
    {
        isNarrow = !isNarrow;
    }

}
