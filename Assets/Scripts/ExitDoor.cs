using UnityEngine;

public class ExitDoor : InteractableObject
{
    public override void Interact(Player player)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextFloor();
        }
    }
}
