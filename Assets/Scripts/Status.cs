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

    void Awake()
    {
        if (hp == 0)
        {
            hp = maxHp;
            mp = maxMp;
        }
    }
}
