using UnityEngine;
using TMPro;
using static EnumData;
using System.Collections.Generic;

public class CurrencyUI : MonoBehaviour
{
    public CurrencyType typeToDisplay = CurrencyType.Gold;
    private TextMeshProUGUI textComponent;
    private Player player;

    private void Start()
    {
        SetupSelf();
    }

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void SetupSelf()
    {
        Player targetPlayer = Player.Instance;

        if (player != null && player.currencies != null)
        {
            player.currencies.OnCurrencyChanged -= UpdateDisplay;
        }

        if (targetPlayer != null && targetPlayer.currencies != null)
        {
            player = targetPlayer;

            player.currencies.OnCurrencyChanged += UpdateDisplay;

            int initialValue = player.currencies.Get(typeToDisplay);
            UpdateText(typeToDisplay, initialValue);
        }
    }

    public void UpdateDisplay(CurrencyType type, int newValue)
    {
        if (type == typeToDisplay)
        {
            UpdateText(type, newValue);
        }
    }

    private void UpdateText(CurrencyType type, int value)
    {
        if (textComponent != null)
        {
            switch (type)
            {
                case CurrencyType.Gold:
                    textComponent.text = $"Gold: {value} G";
                    break;
                case CurrencyType.SoulPoint:
                    textComponent.text = $"Soul: {value} S";
                    break;
                case CurrencyType.UndoPoint:
                    textComponent.text = $"Undo: {value} U";
                    break;
            }
        }
    }
}