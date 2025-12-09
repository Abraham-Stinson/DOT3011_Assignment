using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack/Create An Enemy Attack Pattern and Statistic")]
public class EnemyAttackSO : ScriptableObject
{
    public float damage;
    public AnimatorOverrideController animatorOV;
}
