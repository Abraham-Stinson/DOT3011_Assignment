using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float enemyHealth = 100f;
    void Start()
    {
        
    }

    void Update()
    {

    }
    public void DealDamage(float damage)
    {
        enemyHealth -= damage;
        Debug.Log("Enemy Dealed damage" + damage);
        if (enemyHealth <= 0)
        {
            Debug.Log("Düşman Öldü");
            Destroy(gameObject);
        }
    }
}
