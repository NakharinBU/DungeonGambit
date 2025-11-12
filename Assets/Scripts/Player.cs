using System.Collections;
using UnityEngine;

public class Player : Character
{
    public Inventory inventory = new Inventory();
    public CurrencyManager currencies = new CurrencyManager();
    public bool isAutoMoving = false;
    public TileHighlighter highlighter;

    private void Start()
    {
        base.Start();
        highlighter = GetComponent<TileHighlighter>();
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

        bool moveSuccess = base.Move(direction);

        if (moveSuccess)
        {
            dungeonManager.ResolveEnemyTurn();

            Vector3 targetWorldPos = new Vector3(position.x, position.y, transform.position.z);
            StartCoroutine(MoveSmoothly(targetWorldPos));
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
