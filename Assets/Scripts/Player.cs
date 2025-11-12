using System.Collections;
using UnityEngine;

public class Player : Character
{
    public Inventory inventory = new Inventory();
    public CurrencyManager currencies;
    public bool isAutoMoving = false;
    public TileHighlighter highlighter;
    
    protected override void Awake()
    {
        base.Awake();
        highlighter = GetComponent<TileHighlighter>();
        stats = GetComponent<Status>();
        if (currencies == null)
        {
            currencies = new CurrencyManager();
        }
        if (inventory == null)
        {
            inventory = new Inventory();
        }
    }

    private void Update()
    {
        if (!isAutoMoving)
        {
            if (highlighter != null)
            {
                highlighter.ShowHighlights(this);
            }

            if (dungeonManager != null)
            {
                dungeonManager.ShowAllEnemyIntent(this);
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
        Vector2Int targetPos = position + direction;

        Character targetChar = dungeonManager.GetCharacterAtPosition(targetPos);

        InteractableObject interactableObject = dungeonManager.GetInteractableAtPosition(targetPos) as InteractableObject;

        if (interactableObject != null)
        {
            interactableObject.Interact(this);
        }

        if (targetChar != null && targetChar is Enemy enemy)
        {
            Attack(enemy);

            dungeonManager.ResolveEnemyTurn();

            highlighter?.ClearHighlights();

            return true; // Action สำเร็จแล้ว
        }

        bool moveSuccess = base.Move(direction);

        if (moveSuccess)
        {
            dungeonManager.ResolveEnemyTurn();

            Vector3 targetWorldPos = new Vector3(position.x, position.y, transform.position.z);
            StartCoroutine(MoveSmoothly(targetWorldPos));
        }

        highlighter?.ClearHighlights();

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
