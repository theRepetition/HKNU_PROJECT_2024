using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCHealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab; // HealthBar 프리팹
    public RectTransform healthBarParent; // HealthBar가 생성될 Canvas
    private Slider npcHealthBar; // 생성된 HealthBar의 Slider 컴포넌트
    private Health NPCHealth; // NPC의 Health 컴포넌트
    private Camera mainCamera; // 메인 카메라

    void Start()
    {
        // NPC의 Health 컴포넌트를 가져옴
        NPCHealth = GetComponent<NPCHealth>();

        // 메인 카메라를 참조
        mainCamera = Camera.main;

        // HealthBar를 Canvas에 생성
        GameObject healthBarInstance = Instantiate(healthBarPrefab, healthBarParent);

        // 생성된 HealthBar에서 Slider 컴포넌트를 가져옴
        npcHealthBar = healthBarInstance.GetComponent<Slider>();
    }

    void Update()
    {
        if (npcHealthBar != null)
        {
            // HealthBar의 위치를 NPC 머리 위에 고정되도록 설정
            Vector3 worldPosition = transform.position + Vector3.up * 2.0f; // NPC의 위에 HealthBar 위치
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition); // 화면상의 위치로 변환
            npcHealthBar.transform.position = screenPosition; // HealthBar의 위치 설정

            // HealthBar의 현재 상태를 NPC의 체력에 맞게 업데이트
            npcHealthBar.value = (float)NPCHealth.GetCurrentHealth() / NPCHealth.maxHealth;
        }
    }

    // NPC가 파괴될 때 HealthBar도 제거
    void OnDestroy()
    {
        if (npcHealthBar != null)
        {
            Destroy(npcHealthBar.gameObject); // HealthBar를 삭제
        }
    }
}
