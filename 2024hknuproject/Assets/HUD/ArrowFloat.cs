using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFloat : MonoBehaviour
{
    public Transform target; // 화살표가 따라다닐 오브젝트
    public float floatAmplitude = 0.5f; // 위아래 움직임의 크기
    public float floatSpeed = 1f; // 움직임 속도
    public float offsetY = 1f; // 오브젝트 위에 화살표를 띄울 기본 높이
    private SpriteRenderer spriteRenderer; // 화살표의 SpriteRenderer 컴포넌트

    void Start()
    {
        // SpriteRenderer 컴포넌트를 가져옴
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (target != null)
        {
            // target 위치 기준으로 매 프레임 초기 위치 설정
            Vector3 initialPosition = target.position + Vector3.up * offsetY;

            // 초기 위치 기준으로 위아래로 움직이기
            float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = initialPosition + new Vector3(0, newY, 0);

            // NPC 존재 여부에 따라 투명도 조정
            if (AreNPCsPresent())
            {
                // NPC가 있을 때: 투명도를 높여서 화살표를 보이지 않게 설정
                spriteRenderer.color = new Color(1, 1, 1, 0); // 알파 값 0으로 설정
            }
            else
            {
                // NPC가 없을 때: 투명도를 낮춰서 화살표를 보이게 설정
                spriteRenderer.color = new Color(1, 1, 1, 1); // 알파 값 1으로 설정
            }
        }
    }

    // 현재 씬에 NPC가 있는지 확인하는 메서드
    private bool AreNPCsPresent()
    {
        // 씬에 있는 NPC 오브젝트들을 찾음 ("NPC"는 실제 사용 중인 태그로 변경해야 함)
        return GameObject.FindGameObjectsWithTag("NPC").Length > 0;
    }
}
