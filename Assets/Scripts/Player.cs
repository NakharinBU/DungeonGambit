using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : Character
{
    public static Player Instance { get; private set; }
    public Inventory inventory;
    public CurrencyManager currencies;
    public bool isAutoMoving = false;
    public TileHighlighter highlighter;
    public List<PassiveSkill> passiveSkills = new List<PassiveSkill>();
    public List<ActiveSkill> activeSkills = new List<ActiveSkill>();
    
    public bool isTargeting = false;
    public ActiveSkill currentTargetingSkill { get; private set; } = null; // Skill ที่กำลังถูกเลือกใช้

    public DungeonManager DungeonManagerRef
    {
        get
        {
            return GameManager.Instance?.dungeonManager;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        highlighter = GetComponent<TileHighlighter>();
        stats = GetComponent<Status>();
        inventory = GetComponent<Inventory>();


        if (stats == null) stats = GetComponent<Status>();
        if (currencies == null) currencies = new CurrencyManager();

        if (currencies == null)
        {
            currencies = new CurrencyManager();
        }
        if (inventory == null)
        {
            inventory = new Inventory();
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Player.Instance = this;
        
        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        if (isTargeting)
        {
            HandleSkillTargeting();
            return;
        }

        if (!isAutoMoving)
        {
            if (highlighter != null && DungeonManagerRef != null)
            {
                highlighter.ShowHighlights(this);
            }

            if (DungeonManagerRef != null)
            {
                DungeonManagerRef.ShowAllEnemyIntent(this);
            }


            if (Input.GetMouseButtonDown(0))
            {
                Vector2Int targetPos = GetGridPositionFromMouse();
                Vector2Int direction = targetPos - position;

                if (direction.magnitude > 0 && Mathf.Abs(direction.x) <= 1 && Mathf.Abs(direction.y) <= 1)
                {
                    direction.x = Mathf.Clamp(direction.x, -1, 1);
                    direction.y = Mathf.Clamp(direction.y, -1, 1);
                    Move(direction);
                }
            }
        }
    }

    private Vector2Int GetGridPositionFromMouse()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
    }

    public override bool Move(Vector2Int direction)
    {
        DungeonManager dm = DungeonManager.Instance;
        if (dm == null) return false;


        if (currencies == null) currencies = new CurrencyManager();
        if (inventory == null) inventory = new Inventory();

        Vector2Int targetPos = position + direction;
        Character targetChar = dm.GetCharacterAtPosition(targetPos);
        Tile targetTile = dm.GetTile(targetPos.x, targetPos.y);

        if (targetTile == null || !targetTile.IsWalkable())
        {
            return false; // ติดกำแพง
        }

        if (targetChar != null && targetChar is Enemy enemy)
        {
            Attack(enemy);
            
            if (stats != null)
            {
                RestoreMana(1);
            }

            DungeonManagerRef.ResolveEnemyTurn();

            highlighter?.ClearHighlights();

            return true; // Action สำเร็จแล้ว
        }

        bool moveSuccess = base.Move(direction);

        if (moveSuccess)
        {
            if (stats != null)
            {
                RestoreMana(1);
            }

            CheckPassiveTrigger(EnumData.PassiveTrigger.OnTurnEnd, this);

            InteractableObject interactableObject = dm.GetInteractableAtPosition(targetPos) as InteractableObject;

            if (interactableObject != null)
            {
                interactableObject.Interact(this);
            }

            DungeonManagerRef.ResolveEnemyTurn();

            Vector3 targetWorldPos = new Vector3(position.x, position.y, transform.position.z);
            StartCoroutine(MoveSmoothly(targetWorldPos));
        }

        if (highlighter != null)
        {
            highlighter.ClearHighlights();
        }

        return moveSuccess;
    }

    IEnumerator MoveSmoothly(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.position = target;
    }
    public void CheckPassiveTrigger(EnumData.PassiveTrigger triggerType, Character target, object context = null)
    {
        foreach (PassiveSkill skill in passiveSkills)
        {
            if (skill.trigger == triggerType)
            {
                skill.OnTrigger(this, context);
            }
        }
    }

    private void HandleSkillTargeting()
    {
        if (currentTargetingSkill == null)
        {
            ExitTargetingMode();
            return;
        }

        Vector2Int mouseGridPos = GetGridPositionFromMouse();

        highlighter?.ShowSkillHighlights(currentTargetingSkill, mouseGridPos, position);

        float distance = Vector2Int.Distance(position, mouseGridPos);
        bool inRange = distance <= currentTargetingSkill.range + 0.5f;

        if (Input.GetMouseButtonDown(0))
        {
            if (inRange)
            {

                if (stats.mp >= currentTargetingSkill.mpCost)
                {
                    bool success = currentTargetingSkill.Activate(this, mouseGridPos);
                    if (success)
                    {
                        ExitTargetingMode();
                        DungeonManagerRef?.ResolveEnemyTurn();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[CANCEL] Skill Targeting Canceled by Input.");

            ExitTargetingMode();
        }
    }

    public void EnterTargetingMode(ActiveSkill skill)
    {
        if (skill == null) return;

        isTargeting = true;
        currentTargetingSkill = skill;

        highlighter?.ClearHighlights();

        Debug.Log($"Entering Skill Targeting Mode for: {skill.skillName}");
    }

    public void ExitTargetingMode()
    {
        isTargeting = false;
        currentTargetingSkill = null;
        highlighter?.ClearHighlights();

        highlighter?.ShowHighlights(this);

        Debug.Log("Exiting Skill Targeting Mode.");
    }

    public void AddPassiveSkill(PassiveSkill skill)
    {
        if (skill != null && !passiveSkills.Contains(skill))
        {
            passiveSkills.Add(skill);
            Debug.Log($"Passive Skill '{skill.skillName}' added.");
        }
    }

    public void ReplaceActiveSkill(int slotIndex, ActiveSkill newSkill)
    {
        if (slotIndex < 0) return;

        while (activeSkills.Count <= slotIndex)
        {
            activeSkills.Add(null);
        }

        activeSkills[slotIndex] = newSkill;
        Debug.Log($"Active Skill '{newSkill.skillName}' installed at Slot {slotIndex}.");

        // **FIXME:** เรียก SkillUIManager.Instance.SetupUI(this); เพื่ออัพเดท UI (ถ้ามี)
    }


}
