using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Image revolverImage; // ������ �̹���
    public Image[] bulletSlots; // �Ѿ� ���� �̹��� �迭
    public Sprite bulletSprite; // �Ѿ� ��������Ʈ
    public Sprite emptySlotSprite; // �� ���� ��������Ʈ

    // ���� ������Ʈ �Լ�
    public void UpdateBulletSlots(int bulletsLeft)
    {
        for (int i = 0; i < bulletSlots.Length; i++)
        {
            if (i < bulletsLeft)
            {
                bulletSlots[i].sprite = bulletSprite; // �Ѿ��� ���� ������ �Ѿ� ��������Ʈ ����
            }
            else
            {
                bulletSlots[i].sprite = emptySlotSprite; // �Ѿ��� ������ �� ���� ��������Ʈ ����
            }
        }
    }
}
