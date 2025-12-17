using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager instance { get; private set; }
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    private Image inGameHealthBar;
    private float reduceSpeed = 2f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        inGameHealthBar = UIManager.instance.GetInGameHealthBar();
        Debug.Log($"healthBar UI: {inGameHealthBar.gameObject.name}");
        maxHealth = GameManager.instance.characters[GameManager.instance.currentHeroIndex].playerStatisticsSO.maxHealth;
        health = maxHealth;
        Debug.Log($"PlayerHealthManager: {health}");
    }
    void Update()
    {
        UpdateHealthUI();
    }

    public void ModfiyHealth(float amount)//MUST BE + or -
    {
        Debug.Log($"ModfiyHealth: Player's health has been modified amount: {amount}");
        health += amount;
        ChechkCurrentHealth();
    }

    private void ChechkCurrentHealth()
    {
        if (health <= 0)
        {
            if (LevelManager.instance.isPlayerGetFirstWin)
            {
                Debug.Log("Lose but not at all");

                ResetAllStatsOfPlayer();//RESET ALL STATS
                //LevelManager.instance.BackToTheFormerPosition();
                LevelManager.instance.ReturnFromLevel();
                LevelManager.instance.DestroyCurrentLevel();
                LevelManager.instance.ReturnWithLoseFromLevel();
            }
            else
            {
                KillPlayer();
            }

        }
    }

    public void KillPlayer()
    {
        GameManager.instance.GameOverLose();
    }
    private void UpdateHealthUI()
    {
        if (inGameHealthBar == null) return;

        //Debug.Log(health / maxHealth);
        inGameHealthBar.fillAmount = Mathf.MoveTowards(inGameHealthBar.fillAmount, health / maxHealth, reduceSpeed * Time.deltaTime);
    }
    private void ResetAllStatsOfPlayer()
    {
        health = maxHealth;
        PlayerXpManagement.instance.ResetXp();
        //THERE WILL BE RESET SKILL TREE
    }
}
