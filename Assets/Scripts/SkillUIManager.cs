using System.Collections.Generic;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    public SkillSlotUI slotQ;
    public SkillSlotUI slotW;
    public SkillSlotUI slotE;

    private List<SkillSlotUI> allSlots = new List<SkillSlotUI>();
    private Player currentPlayer;

    // สถานะปัจจุบัน: เลือก Skill หรือรอ Input เดินปกติ
    private ActiveSkill currentTargetingSkill = null;

    private void Start()
    {
        allSlots.Add(slotQ);
        allSlots.Add(slotW);
        allSlots.Add(slotE);

        // กำหนด Hotkey ให้แต่ละ Slot (ถ้ายังไม่ได้กำหนดใน Inspector)
        slotQ.hotkey = KeyCode.Q;
        slotW.hotkey = KeyCode.W;
        slotE.hotkey = KeyCode.E;

        SetupUI(Player.Instance);

        if (Player.Instance == null)
        {
            Debug.LogError("[SETUP] Player.Instance is NULL when SkillUIManager starts.");
        }
    }

    // เมธอดนี้ถูกเรียกใช้เมื่อ Player เปลี่ยน Floor หรือเริ่มเกม
    public void SetupUI(Player player)
    {
        if (player == null) return;

        currentPlayer = player;

        List<ActiveSkill> skills = player.activeSkills;

        if (skills.Count > 0) slotQ.Setup(skills[0]);
        else slotQ.Setup(null);

        if (skills.Count > 1) slotW.Setup(skills[1]);
        else slotW.Setup(null);

        if (skills.Count > 2) slotE.Setup(skills[2]);
        else slotE.Setup(null);
    }

    private void Update()
    {
        if (currentPlayer == null) return;

        if (currentPlayer.isTargeting)
        {
            foreach (var slot in allSlots)
            {
                slot.UpdateState(currentPlayer);
            }
            return;
        }

        foreach (var slot in allSlots)
        {
            slot.HandleInput(currentPlayer);
            slot.UpdateState(currentPlayer);
        }
    }

    // NEW: Logic สำหรับจัดการ Input ขณะที่กำลังเลือกเป้าหมาย
    public void EnterTargetingMode(ActiveSkill skill)
    {
        currentTargetingSkill = skill;
        // 1. **Hide** Highlights ปกติ (Move/Attack)
        // 2. **Show** Highlight ระยะร่าย (Range)
        // 3. **Show** Highlight AOE (ตามเมาส์)
    }

    private void HandleTargetingInput()
    {
        // 1. ตรวจสอบตำแหน่งเมาส์บน Grid
        // Vector2Int mouseGridPos = GetMousePositionOnGrid(); 

        // 2. แสดง Highlight AOE ของ Skill
        // currentPlayer.highlighter.ShowSkillHighlights(currentTargetingSkill, mouseGridPos);

        // 3. ร่าย Skill เมื่อคลิกซ้าย
        if (Input.GetMouseButtonDown(0))
        {
            // bool success = currentTargetingSkill.Activate(currentPlayer, mouseGridPos);
            // if (success)
            // {
            ExitTargetingMode();
            // }
        }

        // 4. ยกเลิกเมื่อคลิกขวาหรือกด Esc
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ExitTargetingMode();
        }
    }

    private void ExitTargetingMode()
    {
        currentTargetingSkill = null;
        // 1. **Clear** Highlight Skill ทั้งหมด
        // 2. **Show** Highlights ปกติ (Move/Attack) อีกครั้ง
        Debug.Log("Exiting Skill Targeting Mode.");
    }
}
