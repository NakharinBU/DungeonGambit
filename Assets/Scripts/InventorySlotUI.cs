using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI stackText;
    public Button useButton;

    private ItemData currentItem;
    private Inventory inventoryRef;

    private int currentStackCount = 0;
    
    private void Start()
    {
        if (useButton != null)
        {
            useButton.onClick.AddListener(UseItem);
        }

        if (Player.Instance != null)
        {
            inventoryRef = Player.Instance.GetComponent<Inventory>();
        }
    }

    public void SetupSlot(ItemData item, int stackCount = 1)
    {
        currentItem = item;
        currentStackCount = stackCount;

        bool hasItem = item != null;

        itemIcon.enabled = hasItem;
        stackText.enabled = hasItem;

        if (hasItem)
        {
            itemIcon.sprite = item.icon;

            if (stackCount > 1)
            {
                stackText.text = stackCount.ToString();
            }
            else
            {
                stackText.text = "";
            }

            if (useButton != null)
            {
                useButton.interactable = item.Type == EnumData.ItemType.Consumable && inventoryRef != null;
            }
        }
        else
        {
            itemIcon.sprite = null;
            stackText.text = "";
            if (useButton != null) useButton.interactable = false;
        }

    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (currentItem != null && currentItem.Type == EnumData.ItemType.Consumable)
            {
                UseItem();
            }
        }
    }*/

    private void UseItem()
    {
        if (currentItem != null && inventoryRef != null)
        {
            currentItem.Use(Player.Instance);

            if (currentItem.Type == EnumData.ItemType.Consumable)
            {
                bool removed = inventoryRef.RemoveItem(currentItem, 1);

                if (removed)
                {
                    InventoryUIManager.Instance?.UpdateInventoryUI();
                }
                else
                {
                    Debug.LogWarning($"Failed to remove {currentItem.Name} from Inventory after use.");
                }
            }
        }
    }
}