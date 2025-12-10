using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PlayerHealthManager : MonoBehaviour
{
    PlayerHealthManager instance;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    private Image inGameHealthBar;
    private float reduceSpeed =2f;
    
    void Awake()
    {
        if(instance == null)
        {
            instance=this;
        }   

        inGameHealthBar=UIManager.instance.GetInGameHealthBar();
        Debug.Log($"healthBar UI: {inGameHealthBar.gameObject.name}");
        maxHealth = GameManager.instance.characters[GameManager.instance.currentHeroIndex].playerStatisticsSO.maxHealth;
        health=maxHealth;
        Debug.Log($"PlayerHealthManager: {health}");
    }
    void Update()
    {
        UpdateHealthUI();
    }

    public void ModfiyHealth(float amount)//MUST BE + or -
    {
        Debug.Log($"ModfiyHealth: Player's health has been modified amount: {amount}");
        health+=amount;
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
    private void UpdateHealthUI()
    {
        if (inGameHealthBar==null) return;
        
        Debug.Log(health / maxHealth);
        inGameHealthBar.fillAmount = Mathf.MoveTowards(inGameHealthBar.fillAmount, health / maxHealth, reduceSpeed * Time.deltaTime);
    }
}
