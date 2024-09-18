using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    public Image[] bulletSlots; // 탄약 슬롯 UI 배열
    public Sprite bulletSprite; // 탄약이 있을 때의 스프라이트
    public Sprite emptySlotSprite; // 빈 슬롯의 스프라이트

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복된 인스턴스가 있으면 파괴
        }
        else
        {
            Instance = this; // 싱글톤 인스턴스 할당
        }
    }

    // 탄약 슬롯을 업데이트하는 함수
    public void UpdateBulletSlots(int bulletsLeft)
    {
        for (int i = 0; i < bulletSlots.Length; i++)
        {
            if (i < bulletsLeft)
            {
                bulletSlots[i].sprite = bulletSprite; // 탄약이 남아있으면 탄약 스프라이트로 설정
            }
            else
            {
                bulletSlots[i].sprite = emptySlotSprite; // 탄약이 없으면 빈 슬롯 스프라이트로 설정
            }
        }
    }
}
