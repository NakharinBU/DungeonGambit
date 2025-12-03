using UnityEngine;
using TMPro;
using static EnumData;
using System.Collections.Generic;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    public CurrencyType typeToDisplay = CurrencyType.Gold;
    private TextMeshProUGUI textComponent;
    private Player player;


    public GameObject goldIcon;
    public GameObject soulIcon;
    public GameObject undoIcon;


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
                    textComponent.text = $"     {value}";
                    break;
                case CurrencyType.SoulPoint:
                    textComponent.text = $"     {value}";
                    break;
                case CurrencyType.UndoPoint:
                    textComponent.text = $"     {value}";
                    break;
            }
        }
    }
}