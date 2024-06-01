using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCHealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab; // HealthBar ������
    public Transform healthBarParent; // HealthBar�� ������ Canvas
    private Slider npcHealthBar; // ������ HealthBar�� Slider ������Ʈ
    private Health NPCHealth; // NPC�� Health ������Ʈ
    private Camera mainCamera; // ���� ī�޶�

    void Start()
    {
        NPCHealth = GetComponent<NPCHealth>();
        mainCamera = Camera.main;

        // HealthBar ���� �� ����
        GameObject healthBarInstance = Instantiate(healthBarPrefab, healthBarParent);
        npcHealthBar = healthBarInstance.GetComponent<Slider>();
    }

    void Update()
    {
        if (npcHealthBar != null)
        {
            // HealthBar ��ġ ������Ʈ
            Vector3 worldPosition = transform.position + Vector3.up * 2.0f; // NPC �Ӹ� ���� �ణ �ø�
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            npcHealthBar.transform.position = screenPosition;

            // HealthBar �� ������Ʈ
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
