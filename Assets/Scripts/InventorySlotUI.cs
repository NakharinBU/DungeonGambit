using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// สืบทอดจาก MonoBehaviour และ IPointerClickHandler
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public TextMeshProUGUI stackText;
    public Button useButton;

    private ItemData currentItem;
    private Inventory inventoryRef;

    private void Start()
    {
        if (useButton != null)
        {
            useButton.onClick.AddListener(UseItem);
        }

        inventoryRef = Player.Instance?.inventory;
    }

    public void SetupSlot(ItemData item, int stackCount = 1)
    {
        currentItem = item;

        bool hasItem = item != null;

        itemIcon.enabled = hasItem;
        if (hasItem)
        {
            itemIcon.sprite = item.icon;
            stackText.text = "";

            if (useButton != null)
            {
                useButton.interactable = item.Type == EnumData.ItemType.Consumable;
            }
        }
        else
        {
            itemIcon.sprite = null;
            stackText.text = "";
            if (useButton != null) useButton.interactable = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        }
    }

    private void UseItem()
    {
        if (currentItem != null && inventoryRef != null)
        {
            currentItem.Use(Player.Instance);

            if (currentItem.Type == EnumData.ItemType.Consumable)
            {
                bool removed = inventoryRef.RemoveItem(currentItem);
                if (removed)
                {
                    InventoryUIManager.Instance?.UpdateInventoryUI();
                }
            }
        }
    }
}