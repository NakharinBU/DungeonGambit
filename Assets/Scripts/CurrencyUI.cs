using UnityEngine;
using TMPro;
using static EnumData;

public class CurrencyUI : MonoBehaviour
{
    public CurrencyType typeToDisplay = CurrencyType.Gold;
    private TextMeshProUGUI textComponent;
    private Player player;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void Setup(Player targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.currencies != null)
        {
            player = targetPlayer;

            // 1. µ‘¥µ“¡ Event
            player.currencies.OnCurrencyChanged += UpdateDisplay;

            // 2. Õ—ª‡¥µ§√—Èß·√°
            int initialValue = player.currencies.Get(typeToDisplay);
            UpdateText(typeToDisplay, initialValue);
        }
    }

    private void UpdateDisplay(CurrencyType type, int newValue)
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
            if (type == CurrencyType.Gold)
            {
                string symbol = (type == CurrencyType.Gold) ? "G" : "N";
                textComponent.text = $"Gold: {value} {symbol}";
            }
            if (type == CurrencyType.SoulPoint)
            {
                string symbol = (type == CurrencyType.SoulPoint) ? "S" : "N";
                textComponent.text = $"Soul: {value} {symbol}";
            }
            if (type == CurrencyType.UndoPoint)
            {
                string symbol = (type == CurrencyType.UndoPoint) ? "U" : "N";
                textComponent.text = $"Undo: {value} {symbol}";
            }
        }
    }

    private void OnDestroy()
    {
        if (player != null && player.currencies != null)
        {
            player.currencies.OnCurrencyChanged -= UpdateDisplay;
        }
    }
}