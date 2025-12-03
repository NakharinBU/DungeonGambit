using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
    public enum StatType { HP, MP, ATK }
    public StatType typeToDisplay = StatType.HP;

    [Header("UI Elements")]
    public TextMeshProUGUI textComponent;

    public Image fillImage;

    private Player player;

    private void Start()
    {
        if (typeToDisplay != StatType.ATK && fillImage == null)
        {
            Debug.LogError($"Fill Image is missing for {typeToDisplay} UI on {gameObject.name}!");
        }

        StartCoroutine(WaitForPlayerSetup());
    }

    public void SetupSelf()
    {
        Player targetPlayer = Player.Instance;

        if (targetPlayer != null)
        {
            if (player != null) UnsubscribeEvents();

            player = targetPlayer;
            SubscribeEvents();

            if (typeToDisplay == StatType.HP)
            {
                UpdateHealth(player.stats.hp, player.stats.maxHp);
            }
            else if (typeToDisplay == StatType.MP)
            {
                UpdateMana(player.stats.mp, player.stats.maxMp);
            }
            else if (typeToDisplay == StatType.ATK)
            {
                UpdateAttack(player.stats.atk);
            }
        }
        else
        {
            Debug.LogError($"Player Instance is null! Cannot setup {typeToDisplay} UI.");
        }
    }

    private void SubscribeEvents()
    {
        if (player == null) return;

        if (typeToDisplay == StatType.HP)
        {
            player.OnHealthChanged += UpdateHealth;
        }
        else if (typeToDisplay == StatType.MP)
        {
            player.OnManaChanged += UpdateMana;
        }
        else if (typeToDisplay == StatType.ATK)
        {
            player.OnAttackChanged += UpdateAttack;
        }

    }

    private void UnsubscribeEvents()
    {
        if (player == null) return;

        player.OnHealthChanged -= UpdateHealth;
        player.OnManaChanged -= UpdateMana;
        player.OnAttackChanged -= UpdateAttack;
    }

    private void UpdateHealth(int current, int max)
    {
        if (textComponent != null)
        {
            textComponent.text = $"HP: {current} / {max}";
        }

        // 2. อัพเดทแถบ (Bar)
        if (fillImage != null && max > 0)
        {
            float fillAmount = (float)current / max;
            fillImage.fillAmount = fillAmount;
        }
    }

    private void UpdateMana(int current, int max)
    {
        // 1. อัพเดท Text
        if (textComponent != null)
        {
            textComponent.text = $"MP: {current} / {max}";
        }

        if (fillImage != null && max > 0)
        {
            float fillAmount = (float)current / max;
            fillImage.fillAmount = fillAmount;
        }
    }

    private void UpdateAttack(int current)
    {
        if (textComponent != null)
        {
            textComponent.text = $"     {current}";
        }
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private IEnumerator WaitForPlayerSetup()
    {
        yield return null;

        while (Player.Instance == null)
        {
            yield return null;
        }

        SetupSelf();
    }
}