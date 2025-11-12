using UnityEngine;

public class CombatSystem : MonoBehaviour
{

    private static CombatSystem _instance;


    public static CombatSystem Instance()
    {
        if (_instance == null)
            _instance = new CombatSystem();
        return _instance;
    }


    private CombatSystem() { }

    public int CalculateDamage(Character attacker, Character defender)
    {
        // int damage = attacker.Status.atk - defender.Status.def; 

        // if (damage < 1) damage = 1; 
        // return damage;
        return 10; // ค่า Mock ไว้ก่อน
    }
}
