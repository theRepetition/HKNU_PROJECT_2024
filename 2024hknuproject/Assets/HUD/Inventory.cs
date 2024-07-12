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
            Debug.LogWarning("�κ��丮�� �ν��Ͻ��� �̹� �����մϴ�.");
            return;
        }
        instance = this;
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 20; // �κ��丮 ����
    private List<Item> items = new List<Item>(); // ������ ����Ʈ

    // ������ �߰� �޼���
    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("�κ��丮�� ������ �����մϴ�.");
            return false;
        }

        items.Add(item);

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }

        return true;
    }

    // ������ ���� �޼���
    public void Remove(Item item)
    {
        int index = items.IndexOf(item);
        if (index != -1)
        {
            items[index] = null; // �������� ������ �����ϴ� ��� null�� ����
        }

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    // �ε����� ������ ���� �޼���
    public void RemoveAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items[index] = null; // �������� ������ �����ϴ� ��� null�� ����

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
        }
    }

    // ������ ����Ʈ�� ��ȯ�ϴ� �޼���
    public List<Item> GetItems()
    {
        return items;
    }
}
