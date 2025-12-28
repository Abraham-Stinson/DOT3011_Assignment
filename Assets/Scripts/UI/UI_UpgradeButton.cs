using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Description Data")]
    [TextArea]
    [SerializeField] private string descriptionText;
    [SerializeField] private int skillCost;

    private string numberColor = "#4AFF4A";

    public string Description => descriptionText;
    public string SkillCost => skillCost.ToString();

    private void Awake()
    {

        ApplyDescription();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UpgradeDescriptionManager.Instance.Show(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpgradeDescriptionManager.Instance.Hide();
    }

    public void ApplyDescription()
    {
        string rawText = descriptionText;

        descriptionText =
            TMPNumberColorizer.ColorizeNumbers(rawText, numberColor);
    }
}

public static class TMPNumberColorizer
{
    // Matches +20%, -5%, +1, 30%, 100 etc.
    private static readonly Regex numberRegex =
        new Regex(@"[+-]?\d+%?", RegexOptions.Compiled);

    public static string ColorizeNumbers(string input, string hexColor)
    {
        return numberRegex.Replace(input, match =>
        {
            return $"<color={hexColor}>{match.Value}</color>";
        });
    }
}
