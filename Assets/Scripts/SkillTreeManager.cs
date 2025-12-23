using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    FlashlightStatsBase flashlightStatsRuntime;

    [SerializeField] private GameObject skillTreeUI;

    private List<string> upgradeCodes = new List<string>
    {
        "WideUpgrade_01",
        "WideUpgrade_02",
        "WideUpgrade_03",
        "WideUpgrade_04",
    };

    private Dictionary<string, int> upgradeCosts;

    private Dictionary<string, System.Action> upgradeActions;

    public int skillPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        flashlightStatsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;

        upgradeActions = new Dictionary<string, System.Action>()
        {
            { "WideUpgrade_01", () => flashlightStatsRuntime.wideDamage *= 1.1f },
            { "WideUpgrade_02", () => flashlightStatsRuntime.wideLifetime *= 1.2f },
            { "WideUpgrade_03", () => flashlightStatsRuntime.wideCooldown /= 1.15f },
            { "WideUpgrade_04", () =>
                { 
                    flashlightStatsRuntime.wideSpeed *= 2f;
                    flashlightStatsRuntime.wideExpansionMultiplier*=2f;
                } 
            },
        };

        upgradeCosts = new Dictionary<string, int>()
        {
            { "WideUpgrade_01", 1 },
            { "WideUpgrade_02", 1 },
            { "WideUpgrade_03", 2 },
            { "WideUpgrade_04", 3 },
        };

        foreach (string code in upgradeCodes)
        {
            LoadUpgrade(code);
        }
        
    }

    private void Update()
    {
        
    }
    public void UpgradeStat(string upgradeName)
    {
        if (!upgradeCosts.ContainsKey(upgradeName))
        {
            Debug.LogWarning($"No cost defined for upgrade: {upgradeName}");
            return;
        }

        int cost = upgradeCosts[upgradeName];

        if (skillPoints < cost)
        {
            Debug.LogWarning(
                $"Not enough skill points for {upgradeName}. " +
                $"Cost: {cost}, Current: {skillPoints}"
            );
            return;
        }

        // Already unlocked check (important)
        if (PlayerPrefs.GetInt(upgradeName, 0) == 1)
        {
            Debug.LogWarning($"{upgradeName} is already unlocked.");
            return;
        }

        // Apply & pay
        skillPoints -= cost;

        ApplyUpgrade(upgradeName);

        SaveUpgrade(upgradeName);

        Debug.Log(
            $"Unlocked {upgradeName} for {cost} points. " +
            $"Remaining points: {skillPoints}"
                 );
    }
    // 1 = true, 0 = false

    private void ApplyUpgrade(string upgradeName)
    {
        if (upgradeActions.ContainsKey(upgradeName))
            upgradeActions[upgradeName].Invoke();
    }
    private void SaveUpgrade(string upgradeName)
    {
        if(!PlayerPrefs.HasKey(upgradeName))
            PlayerPrefs.SetInt(upgradeName, 1);

        PlayerPrefs.Save();
    }

    private void LoadUpgrade(string upgradeName)
    {
        if (PlayerPrefs.GetInt(upgradeName, 0) == 1)
            ApplyUpgrade(upgradeName);
    }

    public void IncreaseSkillPoint(int increaseAmount)
    {
        skillPoints += increaseAmount;
    }
}
