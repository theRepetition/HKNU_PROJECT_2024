using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Image revolverImage; // 리볼버 이미지
    public Image[] bulletSlots; // 총알 슬롯 이미지 배열
    public Sprite bulletSprite; // 총알 스프라이트
    public Sprite emptySlotSprite; // 빈 슬롯 스프라이트

    // 슬롯 업데이트 함수
    public void UpdateBulletSlots(int bulletsLeft)
    {
        for (int i = 0; i < bulletSlots.Length; i++)
        {
            if (i < bulletsLeft)
            {
                bulletSlots[i].sprite = bulletSprite; // 총알이 남아 있으면 총알 스프라이트 설정
            }
            else
            {
                bulletSlots[i].sprite = emptySlotSprite; // 총알이 없으면 빈 슬롯 스프라이트 설정
            }
        }
    }
}
