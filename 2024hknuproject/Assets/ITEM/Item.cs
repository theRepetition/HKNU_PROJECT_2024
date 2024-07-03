using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName; // 아이템 이름
    public string description; // 아이템 설명
    public Sprite icon; // 아이템 아이콘
    public ItemType itemType; // 아이템 타입

    public enum ItemType
    {
        Weapon,
        Ammo,
        Consumable,
        Misc
    }

    // 아이템 사용 메서드
    public virtual void Use()
    {
        Debug.Log(itemName + " 사용됨");
    }
}

[System.Serializable]
public class Weapon : Item
{
    public int damage;
    public float range;

    public override void Use()
    {
        // 무기 사용 로직
        Debug.Log(itemName + " 무기 사용됨");
    }
}

[System.Serializable]
public class Ammo : Item
{
    public int amount;

    public override void Use()
    {
        // 탄약 사용 로직
        Debug.Log(itemName + " 탄약 사용됨");
    }
}

[System.Serializable]
public class Consumable : Item
{
    public int healthRestore;

    public override void Use()
    {
        // 소비 아이템 사용 로직
        Debug.Log(itemName + " 소비 아이템 사용됨");
    }
}
