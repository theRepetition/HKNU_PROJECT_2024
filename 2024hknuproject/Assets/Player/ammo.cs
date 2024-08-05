using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo
{
    public string name;
    public int damage;
    public string effect;

    public Ammo(string name, int damage, string effect)
    {
        this.name = name;
        this.damage = damage;
        this.effect = effect;
    }
}
