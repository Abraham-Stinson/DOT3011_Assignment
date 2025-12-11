using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStatsManager : MonoBehaviour
{
    public static WeaponStatsManager Instance { get; private set; }

    [SerializeField] private FlashlightStatsBase flashlightStatsBase;

    public FlashlightStatsBase flashlightStatsRuntime;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        flashlightStatsRuntime = Instantiate(flashlightStatsBase);
    }
}
