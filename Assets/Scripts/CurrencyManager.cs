using UnityEngine;
using System.Collections.Generic;
using static EnumData;
using System;

[System.Serializable]
public class CurrencyManager
{
    private int gold = 0;
    private int soulPoint = 0;
    private int undoPoint = 0;

    // สมมติว่า SoulUpgrade สร้างทีหลัง
    public List<string> availableUpgrades = new List<string>();


    public event Action<CurrencyType, int> OnCurrencyChanged;


    public void Add(CurrencyType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case CurrencyType.Gold:
                gold += amount;
                OnCurrencyChanged?.Invoke(CurrencyType.Gold, gold);
                Debug.Log($"Added {amount} Gold. Total: {gold}");
                break;
            case CurrencyType.SoulPoint:
                soulPoint += amount;
                Debug.Log($"Added {amount} Soul Points. Total: {soulPoint}");
                break;
            case CurrencyType.UndoPoint:
                undoPoint += amount;
                Debug.Log($"Added {amount} Undo Points. Total: {undoPoint}");
                break;
        }
    }

    public bool Spend(CurrencyType type, int amount)
    {
        if (amount <= 0) return false;

        switch (type)
        {
            case CurrencyType.Gold:
                if (gold >= amount) { gold -= amount; return true; }
                break;
            case CurrencyType.SoulPoint:
                if (soulPoint >= amount) { soulPoint -= amount; return true; }
                break;
            case CurrencyType.UndoPoint:
                if (undoPoint >= amount) { undoPoint -= amount; return true; }
                break;
        }
        Debug.Log($"Failed to spend {amount} of {type}. Not enough currency.");
        return false;
    }

    public int Get(CurrencyType type)
    {
        return type switch
        {
            CurrencyType.Gold => gold,
            CurrencyType.SoulPoint => soulPoint,
            CurrencyType.UndoPoint => undoPoint,
            _ => 0,
        };
    }

    // **NOTE:** เมธอด ApplyUpgrade และ GetAvailableUpgrades (ต้องใช้ SoulUpgrade Class)
}