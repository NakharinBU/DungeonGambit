using UnityEngine;
using static EnumData;

public class Item : MonoBehaviour
{

    private int id;
    private string name;
    private ItemType type;
    private int value;
    private ItemEffect effect;

    public string Name => name;
    public ItemType Type => type;
    public int Value => value;

    public Item(int id, string name, ItemType type, int value, ItemEffect effect)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.value = value;
        this.effect = effect;
    }

    public void Use(Player target)
    {
        if (type != ItemType.Consumable) return;

        switch (effect)
        {
            case ItemEffect.HealHP:

                // target.Heal(value); 
                Debug.Log($"{target.characterName} ใช้ {name} และฟื้นฟู HP {value} หน่วย");
                break;
            case ItemEffect.RestoreMP:
                // target.RestoreMana(value);
                Debug.Log($"{target.characterName} ใช้ {name} และฟื้นฟู MP {value} หน่วย");
                break;

                // ... เพิ่ม ItemEffect อื่นๆ ที่นี่
        }
    }

    public string GetDescription()
    {
        return $"ไอเทมประเภท: {type}, ผลกระทบ: {effect}, ค่า: {value}";
    }
}
