using UnityEngine;
using static EnumData;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "GameData/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public Sprite itemIcon;
    [TextArea] public string description;

    [Header("Pricing")]
    public int price;
    public CurrencyType currencyType = CurrencyType.Gold;

    [Header("Upgrade Logic")]
    public StatusUpgradeType upgradeType;
    public int upgradeAmount;

    [Header("Buy Potion / Item")]
    public ItemData itemToGive;
    public int itemQuantity = 1;

    [Header("Conditions")]
    public bool isOneTimePurchase = true;
}