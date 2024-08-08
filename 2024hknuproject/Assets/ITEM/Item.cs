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
    public int damage;
    public string effect;
    public int quantity; // ���� źȯ ����

    public Ammo(string ammoName, int damage, string effect, Sprite icon, int quantity)
    {
        this.itemName = ammoName;
        this.damage = damage;
        this.effect = effect;
        this.icon = icon;
        this.quantity = quantity;
        this.itemType = ItemType.Ammo; // ������ Ÿ���� źȯ���� ����
    }

    public override void Use()
    {
        if (quantity > 0)
        {
            quantity--;
            Debug.Log($"{itemName} źȯ ����. ���� ����: {quantity}");
        }
        else
        {
            Debug.Log($"{itemName} źȯ�� �����ϴ�.");
        }
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
