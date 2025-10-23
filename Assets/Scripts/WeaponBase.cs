using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public abstract void Fire();
    public abstract void SwitchStance();
}
