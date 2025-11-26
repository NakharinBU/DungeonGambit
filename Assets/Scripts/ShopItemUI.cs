using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    private ShopItemData itemData;
    private ShopManager manager;

    public void Setup(ShopItemData data, ShopManager shopManager)
    {
        itemData = data;
        manager = shopManager;

        iconImage.sprite = data.itemIcon;
        nameText.text = data.itemName;
        costText.text = $"{data.price} {data.currencyType}";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyButtonClicked);
    }

    private void OnBuyButtonClicked()
    {
        if (manager != null && itemData != null)
        {
            manager.BuyItem(itemData);
        }
    }
}