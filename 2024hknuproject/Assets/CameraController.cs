using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f; // 카메라 이동 속도
    public Transform player; // 플레이어 위치 참조
    public CanvasGroup fadeCanvasGroup; // 페이드 인/아웃을 위한 CanvasGroup
    public float fadeDuration = 0.5f; // 페이드 효과 지속 시간
    public PlayerMovement playermovement; //이동 제한 관리
    private Vector3 offset; // 플레이어와 카메라 사이의 거리
    private bool isTransitioning = false; // 카메라가 이동 중인지 여부
    private bool shouldFollowPlayer = true; // 카메라가 플레이어를 따라가는지 여부
    public CycleManager cycleManager; // CycleManager에 접근하기 위한 참조
    void Start()
    {
        // 플레이어 오브젝트가 설정되지 않았다면 자동으로 찾음
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        offset = transform.position - player.position;

        // 게임 시작 시 알파값을 0으로 설정하여 화면이 밝은 상태로 시작
        fadeCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (isTransitioning || !shouldFollowPlayer)
            return;

        // 플레이어를 부드럽게 따라가는 부분
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }

    // 트리거가 발동되면 방 이동을 처리하는 함수
    public void TriggerRoomTransition()
    {
        StartCoroutine(HandleRoomTransition());
    }

    // 암전 및 카메라 동작을 처리하는 코루틴
    private IEnumerator HandleRoomTransition()
    {
        isTransitioning = true;  // 카메라 이동 중
        shouldFollowPlayer = false;  // 카메라가 플레이어 추적 중지

        // 1. 페이드 아웃 (화면을 어둡게 만듦)
        yield return StartCoroutine(FadeOut());
        // 2. 카메라 또는 플레이어 이동
        // 이 부분에서 카메라 이동이나 플레이어 위치를 전환
        // 예: transform.position = 새로운 방 위치;
        yield return new WaitForSeconds(0.5f); // 이동하는 동안 잠시 기다림
        playermovement.DisableMovement();//플레이어 이동 멈춤
        shouldFollowPlayer = true;  // 카메라가 다시 플레이어를 추적
        isTransitioning = false;  // 카메라 이동 종료
        yield return new WaitForSeconds(0.5f); // 이동하는 동안 잠시 기다림
        // 3. 페이드 인 (화면을 다시 밝게 만듦)
        yield return StartCoroutine(FadeIn());
        playermovement.EnableMovement();// 이동활성화
        cycleManager.SpawnNPCs();// CycleManager의 SpawnNPCs() 함수 호출하여 NPC 소환


    }

    // 화면을 어둡게 만드는 함수 (페이드 아웃)
    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);  // Alpha 값을 0에서 1로 변화
            yield return null;
        }
    }

    // 화면을 밝게 만드는 함수 (페이드 인)
    private IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);  // Alpha 값을 1에서 0으로 변화
            yield return null;
        }
    }
}
