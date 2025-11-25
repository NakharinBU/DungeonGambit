using UnityEngine;

public abstract class PassiveSkill : SkillBase
{
    public EnumData.PassiveTrigger trigger;

    // เมธอดสำหรับเรียกใช้ทันทีที่ Skill ถูกติดตั้ง (เช่น เพิ่ม Max HP)
    public virtual void ApplyOnAcquire(Player player) { }

    // เมธอดที่ถูกเรียกเมื่อสิ้นสุดตา
    public virtual void OnTurnEnd(Player player) { }

    // เมธอดที่ถูกเรียกเมื่อเกิด Trigger (เช่น การฆ่ามอนสเตอร์)
    public virtual void OnTrigger(Player player, object context = null) { }
}
