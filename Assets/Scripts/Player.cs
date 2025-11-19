using System.Collections;
using UnityEngine;

public class Player : Character
{
    public static Player Instance { get; private set; }
    public Inventory inventory;
    public CurrencyManager currencies;
    public bool isAutoMoving = false;
    public TileHighlighter highlighter;

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

        Instance = this;

        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        if (!isAutoMoving)
        {
            if (highlighter != null && DungeonManagerRef != null)
            {
                highlighter.ShowHighlights(this); // Tile Highlight
            }

            // **FIX 4: สั่งแสดง Enemy Intent**
            if (DungeonManagerRef != null)
            {
                DungeonManagerRef.ShowAllEnemyIntent(this); // Enemy Intent (ทำให้ Enemy "แสดง" การเดิน)
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

            DungeonManagerRef.ResolveEnemyTurn();

            highlighter?.ClearHighlights();

            return true; // Action สำเร็จแล้ว
        }

        bool moveSuccess = base.Move(direction);

        if (moveSuccess)
        {
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
}
