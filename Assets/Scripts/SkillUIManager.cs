using System.Collections.Generic;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance { get; private set; }

    public SkillSlotUI slotQ;
    public SkillSlotUI slotW;
    public SkillSlotUI slotE;

    private List<SkillSlotUI> allSlots = new List<SkillSlotUI>();
    private Player currentPlayer;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        allSlots.Add(slotQ);
        allSlots.Add(slotW);
        allSlots.Add(slotE);

        slotQ.hotkey = KeyCode.Q;
        slotW.hotkey = KeyCode.W;
        slotE.hotkey = KeyCode.E;

        SetupUI(Player.Instance);

        if (Player.Instance == null)
        {
            Debug.LogError("[SETUP] Player.Instance is NULL when SkillUIManager starts.");
        }
    }

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
}
