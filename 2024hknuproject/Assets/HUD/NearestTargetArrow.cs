using UnityEngine;
using UnityEngine.UI;

public class NearestTargetArrow : MonoBehaviour
{
    public Transform player;
    public RectTransform arrowUI;
    public string targetTag; // 타겟 오브젝트의 태그
    public Canvas canvas; // Screen Space - Camera 모드로 설정된 캔버스
    public float offsetY = 50f; // 오브젝트 위에 화살표를 띄울 오프셋

    private Camera mainCamera;

    void Start()
    {
        mainCamera = canvas.worldCamera; // 캔버스에 연결된 카메라 사용
    }

    void Update()
    {
        // 가장 가까운 타겟 오브젝트 찾기
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        Transform nearestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(player.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestTarget = target.transform;
            }
        }

        if (nearestTarget != null)
        {
            // 타겟의 화면 좌표 계산
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(nearestTarget.position);

            // 타겟이 화면 밖에 있을 때
            if (screenPoint.z > 0 && (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1))
            {
                arrowUI.gameObject.SetActive(true);

                // 화면 경계 위치 계산
                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    mainCamera.ViewportToScreenPoint(GetEdgePosition(screenPoint)),
                    mainCamera,
                    out canvasPos
                );

                arrowUI.localPosition = canvasPos;

                // 방향에 따라 화살표 회전
                Vector3 direction = nearestTarget.position - player.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                arrowUI.rotation = Quaternion.Euler(0, 0, angle);
            }
            // 타겟이 화면 안에 있을 때
            else if (screenPoint.z > 0)
            {
                arrowUI.gameObject.SetActive(true);

                // 오브젝트의 위쪽에 화살표 위치
                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    mainCamera.WorldToScreenPoint(nearestTarget.position + Vector3.up * offsetY),
                    mainCamera,
                    out canvasPos
                );

                arrowUI.localPosition = canvasPos;

                // 화살표를 아래로 가리키도록 90도 회전
                arrowUI.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
        else
        {
            arrowUI.gameObject.SetActive(false); // 타겟이 없으면 화살표 숨김
        }
    }

    // 화면 경계 위치 계산 함수
    private Vector3 GetEdgePosition(Vector3 screenPoint)
    {
        Vector3 edgePosition = screenPoint;

        // 가로 경계에 위치할 경우
        if (Mathf.Abs(screenPoint.x - 0.5f) > Mathf.Abs(screenPoint.y - 0.5f))
        {
            edgePosition.x = screenPoint.x < 0.5f ? 0 : 1;
            edgePosition.y = Mathf.Clamp01(screenPoint.y);
        }
        // 세로 경계에 위치할 경우
        else
        {
            edgePosition.y = screenPoint.y < 0.5f ? 0 : 1;
            edgePosition.x = Mathf.Clamp01(screenPoint.x);
        }

        return edgePosition;
    }
}
