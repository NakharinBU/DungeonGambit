using UnityEngine;

public class TrappedMerchant : InteractableObject
{
    public GameObject visualCue;

    private bool isFreed = false;

    public void FreeMerchant()
    {
        isFreed = true;
        Debug.Log("Merchant is now free and open for business!");
        if (visualCue != null) visualCue.SetActive(true);
    }

    public override void Interact(Player player)
    {
        if (!isFreed)
        {
            Debug.Log("Merchant: Help me first! Defeat all the enemies!");
            return;
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OpenShopUI();
        }
        else
        {
            Debug.LogError("ShopManager not assigned to TrappedMerchant!");
        }
    }
}