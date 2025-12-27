using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    FlashlightStatsBase flashlightStatsRuntime;
    NightStickStatsBase nightStickStatsRuntime;

    [SerializeField] private GameObject nightStickTreePanel;
    [SerializeField] private GameObject flashlightTreePanel;

    //[SerializeField] private GameObject skillTreeUI;

    private List<string> upgradeCodes = new List<string>
    {
        "WideUpgrade_01",
        "WideUpgrade_02",
        "WideUpgrade_03",
        "WideUpgrade_04",
        //Night Stick
        "StickUpgrade_Damage",
        "StickUpgrade_Range",
        "StickUpgrade_Combo",
    };

    private Dictionary<string, int> upgradeCosts;

    private Dictionary<string, System.Action> upgradeActions;
    private List<string> flashlightUpgrades = new List<string>
    {
        "WideUpgrade_01", "WideUpgrade_02", "WideUpgrade_03", "WideUpgrade_04"
    };
    private List<string> nightStickUpgrades = new List<string>
    {
        "StickUpgrade_Damage", "StickUpgrade_Combo"
    };

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
        if (WeaponStatsManager.Instance != null)
        {
            flashlightStatsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;
            nightStickStatsRuntime = WeaponStatsManager.Instance.nightStickStatsRuntime;
        }
        flashlightStatsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;

        upgradeActions = new Dictionary<string, System.Action>()
        {
            // --- FLASHLIGHT ---
            { "WideUpgrade_01", () => flashlightStatsRuntime.wideDamage *= 1.1f },
            { "WideUpgrade_02", () => flashlightStatsRuntime.wideLifetime *= 1.2f },
            { "WideUpgrade_03", () => flashlightStatsRuntime.wideCooldown /= 1.15f },
            { "WideUpgrade_04", () =>
                { 
                    flashlightStatsRuntime.wideSpeed *= 2f;
                    flashlightStatsRuntime.wideExpansionMultiplier*=2f;
                } 
            },
            
            // --- NIGHTSTICK ---
            { "StickUpgrade_Damage", () => nightStickStatsRuntime.damageMultiply *= 1.2f },
            { "StickUpgrade_Combo",  () => Debug.Log("Combo Upgrade") }
        };

        upgradeCosts = new Dictionary<string, int>()
        {
            { "WideUpgrade_01", 1 },
            { "WideUpgrade_02", 1 },
            { "WideUpgrade_03", 2 },
            { "WideUpgrade_04", 3 },

            { "StickUpgrade_Damage", 1 },
            { "StickUpgrade_Combo", 1 },
        };

        foreach (string code in flashlightUpgrades) LoadUpgrade(code);
        foreach (string code in nightStickUpgrades) LoadUpgrade(code);


        UpdateSkillTreeUI();

    }

    private void Update()
    {

    }

    private void UpdateSkillTreeUI()
    {
        if (nightStickTreePanel != null) nightStickTreePanel.SetActive(false);
        if (flashlightTreePanel != null) flashlightTreePanel.SetActive(false);

        int currentHeroIndex = GameManager.instance.currentHeroIndex;

        switch (currentHeroIndex)
        {
            case 0: // Warrior - NightStick
                if (nightStickTreePanel != null) nightStickTreePanel.SetActive(true);
                break;

            case 1: // Mage - Beam?
                    // if (mageTreePanel != null) mageTreePanel.SetActive(true);
                break;

            case 2: // Ranger - Flashlight
                if (flashlightTreePanel != null) flashlightTreePanel.SetActive(true);
                break;

            default:
                Debug.LogWarning("SkillTreeManager: Tanımlanmamış karakter indexi!");
                break;
        }
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
        {
            upgradeActions[upgradeName].Invoke();
            Debug.Log(upgradeName + " yeteneği aktif edildi.");
        }
    }
    private void SaveUpgrade(string upgradeName)
    {
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
