using UnityEngine;
using static EnumData;

public class Item : MonoBehaviour
{
    // --- Properties (ตาม Diagram) ---
    private int id;                 // - id : int
    private string name;            // - name : string
    private ItemType type;          // - type : ItemType
    private int value;              // - value : int (e.g., heal amount, selling price)
    private ItemEffect effect;      // - effect : ItemEffect

    // Properties สำหรับการอ่านค่าภายนอก
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

    // --- Methods (ตาม Diagram) ---

    // + Use(target : Player) : void
    public void Use(Player target)
    {
        if (type != ItemType.Consumable) return;

        switch (effect)
        {
            case ItemEffect.HealHP:
                // สมมติว่า Player มี method Heal() (เราจะสร้างใน Wave 3)
                // target.Heal(value); 
                //Debug.Log($"{target.Name} ใช้ {name} และฟื้นฟู HP {value} หน่วย");
                break;
            case ItemEffect.RestoreMP:
                // สมมติว่า Player มี method RestoreMana() (เราจะสร้างใน Wave 3)
                // target.RestoreMana(value);
                //Debug.Log($"{target.Name} ใช้ {name} และฟื้นฟู MP {value} หน่วย");
                break;
                // ... เพิ่ม ItemEffect อื่นๆ ที่นี่
        }
    }

    // + GetDescription() : string
    public string GetDescription()
    {
        return $"ไอเทมประเภท: {type}, ผลกระทบ: {effect}, ค่า: {value}";
    }
}
