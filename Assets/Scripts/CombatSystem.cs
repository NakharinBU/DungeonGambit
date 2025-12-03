using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    public GameObject basicAttackVFXPrefab;
    public float vfxDuration = 0.3f;


    public AudioClip sfxClip;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ProcessAttack(Character attacker, Character target)
    {
        if (target.stats.hp <= 0) return;

        int damage = attacker.stats.atk;

        if (basicAttackVFXPrefab != null && DungeonManager.Instance != null)
        {
            DungeonManager.Instance.PlayVFX(basicAttackVFXPrefab, target.position, vfxDuration);
        }

        if (sfxClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxClip, sfxVolume);
        }

        target.TakeDamage(damage);

        Debug.Log($"{attacker.characterName} attacked {target.characterName} for {damage} damage.");
        
    }
}