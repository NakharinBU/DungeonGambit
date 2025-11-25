using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static EnumData;

public class SkillCardUI : MonoBehaviour
{
    public Button selectButton;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Image iconImage;

    private SkillCardData currentCard;

    private void Start()
    {
        selectButton.onClick.AddListener(OnCardSelected);
    }

    public void SetupCard(SkillCardData card, int playerSouls)
    {
        currentCard = card;
        gameObject.SetActive(true);

        if (card.skillAsset == null)
        {
            Debug.LogError($"Skill Asset is missing for Card: {card.cardName}. Fix the ScriptableObject Asset!", card);
            nameText.text = "ERROR: Missing Skill";
            iconImage.sprite = null;
            selectButton.interactable = false;
            return;
        }

        nameText.text = card.skillAsset.skillName;
        costText.text = card.soulCost.ToString();
        iconImage.sprite = card.skillAsset.icon;

        bool canAfford = playerSouls >= card.soulCost;
        selectButton.interactable = canAfford;
        costText.color = canAfford ? Color.white : Color.red;
    }

    private void OnCardSelected()
    {
        if (currentCard != null)
        {
            SkillUpgrade.Instance.SelectCard(currentCard);
        }
    }
}