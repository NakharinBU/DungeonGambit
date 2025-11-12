using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public Vector2Int position;
    protected DungeonManager dungeonManager;

    protected virtual void Awake()
    {
        
    }

    public void Initialize(DungeonManager manager)
    {
        dungeonManager = manager;
        position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    public abstract void Interact(Player player);

    protected void DestroySelf()
    {
        if (dungeonManager != null)
        {
            dungeonManager.RemoveInteractable(this);
        }
        Destroy(gameObject);
    }
}