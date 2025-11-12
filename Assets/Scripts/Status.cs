using System;
using UnityEngine;

[Serializable]
public class Status : MonoBehaviour
{
    public int hp;
    public int maxHp;
    public int mp;
    public int maxMp;
    public int atk;

    public Status(int health, int mana, int attack, int defense)
    {
        maxHp = health;
        hp = maxHp;
        maxMp = mana;
        mp = maxMp;
        atk = attack;
    }

}
