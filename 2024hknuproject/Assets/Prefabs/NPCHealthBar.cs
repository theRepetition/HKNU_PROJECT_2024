using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCHealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab; // HealthBar 프리팹
    public Transform healthBarParent; // HealthBar를 포함할 Canvas
    private Slider npcHealthBar; // 생성된 HealthBar의 Slider 컴포넌트
    private Health NPCHealth; // NPC의 Health 컴포넌트
    private Camera mainCamera; // 메인 카메라

    void Start()
    {
        NPCHealth = GetComponent<NPCHealth>();
        mainCamera = Camera.main;

        // HealthBar 생성 및 설정
        GameObject healthBarInstance = Instantiate(healthBarPrefab, healthBarParent);
        npcHealthBar = healthBarInstance.GetComponent<Slider>();
    }

    void Update()
    {
        if (npcHealthBar != null)
        {
            // HealthBar 위치 업데이트
            Vector3 worldPosition = transform.position + Vector3.up * 2.0f; // NPC 머리 위로 약간 올림
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            npcHealthBar.transform.position = screenPosition;

            // HealthBar 값 업데이트
            npcHealthBar.value = (float)NPCHealth.GetCurrentHealth() / NPCHealth.maxHealth;
        }
    }

    void OnDestroy()
    {
        if (npcHealthBar != null)
        {
            Destroy(npcHealthBar.gameObject);
        }
    }
}
