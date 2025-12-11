using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeManager : MonoBehaviour
{
    FlashlightStatsBase flashlightStatsRuntime;

    [SerializeField] private GameObject skillTreeUI;

    private List<string> upgradeCodes = new List<string>
    {
        "WideUpgrade_01",
        "WideUpgrade_02",
        "WideUpgrade_03",
        "WideUpgrade_04",
    };

    private Dictionary<string, System.Action> upgradeActions;


    private void Start()
    {
        flashlightStatsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;

        upgradeActions = new Dictionary<string, System.Action>()
        {
            { "WideUpgrade_01", () => flashlightStatsRuntime.wideDamage *= 1.1f },
            { "WideUpgrade_02", () => flashlightStatsRuntime.wideLifetime *= 1.2f },
            { "WideUpgrade_03", () => flashlightStatsRuntime.wideCooldown /= 1.15f },
            { "WideUpgrade_04", () => flashlightStatsRuntime.wideSpeed *= 2f },
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
        ApplyUpgrade(upgradeName);

        SaveUpgrade(upgradeName);
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
}
