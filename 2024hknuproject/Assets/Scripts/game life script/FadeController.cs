using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // CanvasGroup을 할당

    public float fadeDuration = 1f; // 페이드 인/아웃 시간

    private void Start()
    {
        // 처음에 페이드 아웃 (화면이 완전히 밝아진 상태)
        fadeCanvasGroup.alpha = 0f;
    }

    // 화면을 어둡게 만드는 함수 (페이드 아웃)
    public void FadeOut()
    {
        StartCoroutine(Fade(1f)); // Alpha 값을 1로 페이드
    }

    // 화면을 밝게 만드는 함수 (페이드 인)
    public void FadeIn()
    {
        StartCoroutine(Fade(0f)); // Alpha 값을 0으로 페이드
    }

    // Alpha 값을 서서히 변경하는 코루틴
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha; // 페이드 완료 후 알파 값 고정
    }
}
