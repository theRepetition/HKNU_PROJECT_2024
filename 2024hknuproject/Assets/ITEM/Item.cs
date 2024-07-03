using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName; // ������ �̸�
    public string description; // ������ ����
    public Sprite icon; // ������ ������
    public ItemType itemType; // ������ Ÿ��

    public enum ItemType
    {
        Weapon,
        Ammo,
        Consumable,
        Misc
    }

    // ������ ��� �޼���
    public virtual void Use()
    {
        Debug.Log(itemName + " ����");
    }
}

[System.Serializable]
public class Weapon : Item
{
    public int damage;
    public float range;

    public override void Use()
    {
        // ���� ��� ����
        Debug.Log(itemName + " ���� ����");
    }
}

[System.Serializable]
public class Ammo : Item
{
    public int amount;

    public override void Use()
    {
        // ź�� ��� ����
        Debug.Log(itemName + " ź�� ����");
    }
}

[System.Serializable]
public class Consumable : Item
{
    public int healthRestore;

    public override void Use()
    {
        // �Һ� ������ ��� ����
        Debug.Log(itemName + " �Һ� ������ ����");
    }
}
