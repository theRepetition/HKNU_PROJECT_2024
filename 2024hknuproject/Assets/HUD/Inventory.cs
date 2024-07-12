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
        int index = items.IndexOf(item);
        if (index != -1)
        {
            items[index] = null; // 아이템을 실제로 제거하는 대신 null로 설정
        }

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    // 인덱스로 아이템 제거 메서드
    public void RemoveAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items[index] = null; // 아이템을 실제로 제거하는 대신 null로 설정

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
        }
    }

    // 아이템 리스트를 반환하는 메서드
    public List<Item> GetItems()
    {
        return items;
    }
}
