using UnityEngine;
using static EnumData;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/Item Data")]
public abstract class ItemData : ScriptableObject
{
    [SerializeField] private int id;
    public string itemName;
    public ItemType type;
    public int value;
    public ItemEffect effect;
    public Sprite icon;

    public string Name => itemName;
    public ItemType Type => type;
    public int Value => value;

    public virtual void Use(Player target)
    {
        if (type != ItemType.Consumable) return;

        switch (effect)
        {
            case ItemEffect.HealHP:

                target.Heal(value); 
                Debug.Log($"{target.characterName} ใช้ {name} และฟื้นฟู HP {value} หน่วย");
                break;
            case ItemEffect.RestoreMP:
                target.RestoreMana(value);
                Debug.Log($"{target.characterName} ใช้ {name} และฟื้นฟู MP {value} หน่วย");
                break;
        }
    }

    public string GetDescription()
    {
        return $"ไอเทมประเภท: {type}, ผลกระทบ: {effect}, ค่า: {value}";
    }
}
