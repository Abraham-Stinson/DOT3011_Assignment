using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    PlayerHealthManager instance;
    [SerializeField] private float health;
    
    void Awake()
    {
        if(instance == null)
        {
            instance=this;
        }   

        health = GameManager.instance.characters[GameManager.instance.currentHeroIndex].playerStatisticsSO.maxHealth;
        Debug.Log($"PlayerHealthManager: {health}");
    }

    public void ModfiyHealth(float amount)//MUST BE + or -
    {
        if (amount > 0)
        {
            health+=amount;
        }
        else
        {
            health-=amount;
        }
        ChechkCurrentHealth();
    }

    private void ChechkCurrentHealth(){
        if (health <= 0)
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        GameManager.instance.GameOverLose();
    }
}
