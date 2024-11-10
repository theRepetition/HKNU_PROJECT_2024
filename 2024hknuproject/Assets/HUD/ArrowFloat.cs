using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFloat : MonoBehaviour
{
    public Transform target; // 화살표가 따라다닐 오브젝트
    public float floatAmplitude = 0.5f; // 위아래 움직임의 크기
    public float floatSpeed = 1f; // 움직임 속도
    public float offsetY = 1f; // 오브젝트 위에 화살표를 띄울 기본 높이

    void Update()
    {
        if (target != null)
        {
            // target 위치 기준으로 매 프레임 초기 위치 설정
            Vector3 initialPosition = target.position + Vector3.up * offsetY;

            // 초기 위치 기준으로 위아래로 움직이기
            float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = initialPosition + new Vector3(0, newY, 0);
        }
    }
}
