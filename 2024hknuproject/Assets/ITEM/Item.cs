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
        Ammo,
        HealthRecovery,  // 즉시 체력 회복 아이템
        HealthBoost,     // 최대 체력 증가 아이템
        ActionPointBoost // 최대 행동력 증가 아이템 
    }

    // 아이템 사용 메서드
    public virtual void Use()
    {
        Debug.Log(itemName + " 사용");
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
        Debug.Log(itemName + " 무기 사용");
    }
}

[System.Serializable]
public class Ammo : Item
{
    public int damage;
    public string effect;
    public int quantity; // 남은 탄환 수

    public Ammo(string ammoName, int damage, string effect, Sprite icon, int quantity)
    {
        this.itemName = ammoName;
        this.damage = damage;
        this.effect = effect;
        this.icon = icon;
        this.quantity = quantity;
        this.itemType = ItemType.Ammo; // 아이템 타입을 탄환으로 설정
    }

    public override void Use()
    {
        if (quantity > 0)
        {
            quantity--;
            Debug.Log($"{itemName} 탄환 사용. 남은 탄환: {quantity}");
        }
        else
        {
            Debug.Log($"{itemName} 탄환이 없습니다.");
        }
    }
}
[System.Serializable]
public class HealthRecovery : Item
{
    public int healthRestore;

    public HealthRecovery(string name, int restoreAmount, Sprite icon)
    {
        itemName = name;
        healthRestore = restoreAmount;
        this.icon = icon;
        itemType = ItemType.HealthRecovery;
    }

    public override void Use()
    {
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healthRestore);
            Debug.Log($"{itemName} 사용: 체력 {healthRestore} 회복");
        }
    }
}
[System.Serializable]
public class HealthBoost : Item
{
    public int maxHealthIncrease;

    public HealthBoost(string name, int maxIncrease, Sprite icon)
    {
        itemName = name;
        maxHealthIncrease = maxIncrease;
        this.icon = icon;
        itemType = ItemType.HealthBoost;
    }

    public override void Use()
    {
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.IncreaseMaxHealth(maxHealthIncrease);
            Debug.Log($"{itemName} 사용: 최대 체력 {maxHealthIncrease} 증가");
        }
    }
}
[System.Serializable]
public class ActionPointBoost : Item
{
    public int maxActionPointIncrease;

    public ActionPointBoost(string name, int maxIncrease, Sprite icon)
    {
        itemName = name;
        maxActionPointIncrease = maxIncrease;
        this.icon = icon;
        itemType = ItemType.ActionPointBoost;
    }

    public override void Use()
    {
        PlayerCombat playerCombat = GameObject.FindObjectOfType<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.IncreaseMaxActionPoints(maxActionPointIncrease);
            Debug.Log($"{itemName} 사용: 최대 행동력 {maxActionPointIncrease} 증가");
        }
    }
}

