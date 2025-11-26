using UnityEngine;
using System.Collections.Generic;
using static EnumData;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject shopPanel;
    public Transform itemSlotParent;
    public ShopItemUI itemUIPrefab;

    [Header("Data")]
    public List<ShopItemData> availableItems;
    private Player currentPlayer;

    private HashSet<ShopItemData> purchasedItems = new HashSet<ShopItemData>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void OpenShopUI()
    {
        currentPlayer = Player.Instance;
        if (currentPlayer == null) return;

        ClearShopUI();

        foreach (var item in availableItems)
        {
            if (item.isOneTimePurchase && purchasedItems.Contains(item)) continue;

            ShopItemUI uiSlot = Instantiate(itemUIPrefab, itemSlotParent);
            uiSlot.Setup(item, this);
        }

        shopPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseShopUI()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BuyItem(ShopItemData itemData)
    {
        if (currentPlayer == null) return;

        if (currentPlayer.currencies.Get(itemData.currencyType) < itemData.price)
        {
            Debug.Log("Failed to buy: Not enough money!");
            return;
        }

        bool success = currentPlayer.currencies.Spend(itemData.currencyType, itemData.price);

        if (success)
        {
            if (itemData.itemToGive != null)
            {
                Inventory playerInventory = currentPlayer.GetComponent<Inventory>();

                if (playerInventory != null)
                {
                    playerInventory.AddItem(itemData.itemToGive, itemData.itemQuantity);
                    Debug.Log($"Player bought {itemData.itemQuantity}x {itemData.itemToGive.Name}.");
                }
                else
                {
                    Debug.LogError("Player does not have an Inventory component to receive the purchased item!");
                }
            }
            // 🎯 2. ให้ Status Upgrade (ถ้ามี)
            else if (itemData.upgradeType != StatusUpgradeType.None)
            {
                ApplyStatusUpgrade(itemData);
            }
            else
            {
                Debug.LogWarning($"Item {itemData.itemName} purchased, but neither an Item nor a Status Upgrade was applied.");
            }


            // 3. จัดการ One-Time Purchase
            if (itemData.isOneTimePurchase)
            {
                purchasedItems.Add(itemData);
                OpenShopUI();
            }
            else
            {
                Debug.Log($"Item {itemData.itemName} purchased successfully, multiple times allowed.");
            }
        }
    }

    private void ApplyStatusUpgrade(ShopItemData itemData)
    {
        switch (itemData.upgradeType)
        {
            case StatusUpgradeType.MaxHP:
                currentPlayer.UpgradeMaxHP(itemData.upgradeAmount);
                break;
            case StatusUpgradeType.MaxMP:
                currentPlayer.UpgradeMaxMP(itemData.upgradeAmount);
                break;
            case StatusUpgradeType.Attack:
                currentPlayer.UpgradeBaseAttack(itemData.upgradeAmount);
                break;
        }
    }

    private void ClearShopUI()
    {
        foreach (Transform child in itemSlotParent)
        {
            Destroy(child.gameObject);
        }
    }
}