using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("인벤토리에 인스턴스가 이미 존재합니다.");
            return;
        }
        instance = this;
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 20; // 인벤토리 공간
    private List<Item> items = new List<Item>(); // 아이템 리스트

    // 아이템 추가 메서드
    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("인벤토리에 공간이 부족합니다.");
            return false;
        }

        items.Add(item);

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }

        return true;
    }

    // 아이템 제거 메서드
    public void Remove(Item item)
    {
        items.Remove(item);

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    // 아이템 리스트를 반환하는 메서드
    public List<Item> GetItems()
    {
        return items;
    }
}
