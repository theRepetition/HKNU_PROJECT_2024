using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D를 사용하기 위한 네임스페이스

public class FlickeringLight : MonoBehaviour
{
    private Light2D lightSource; // 2D 라이트
    public float minIntensity = 0.5f; // 최소 밝기
    public float maxIntensity = 1.5f; // 최대 밝기
    public float flickerSpeed = 0.1f; // 깜빡이는 속도

    void Start()
    {
       
            lightSource = GetComponentInChildren<Light2D>();
    }

    void Update()
    {
        float targetIntensity = Random.Range(minIntensity, maxIntensity);
        lightSource.intensity = Mathf.Lerp(lightSource.intensity, targetIntensity, flickerSpeed * Time.deltaTime);
    }
}
