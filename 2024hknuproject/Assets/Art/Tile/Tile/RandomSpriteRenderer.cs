using UnityEngine;

public class MultipleRandomOverlays : MonoBehaviour
{
    public SpriteRenderer mainSpriteRenderer; // 메인 스프라이트 렌더러
    public Sprite[] overlaySprites; // 덧씌울 스프라이트들 (얼룩, 금 등)
    public int overlayCount = 3; // 덧씌울 오버레이 수
    public Vector2 randomScaleRange = new Vector2(0.8f, 1.2f); // 크기 랜덤화 범위
    public Vector2 randomRotationRange = new Vector2(-15f, 15f); // 회전 랜덤화 범위
    public float overlayOpacity = 0.5f; // 오버레이 투명도
    public Vector2 tileSize = new Vector2(1f, 1f); // 타일 크기 (예: 1x1 유닛)

    void Start()
    {
        for (int i = 0; i < overlayCount; i++)
        {
            ApplyRandomOverlay();
        }
    }

    void ApplyRandomOverlay()
    {
        if (overlaySprites.Length > 0)
        {
            int randomIndex = Random.Range(0, overlaySprites.Length);
            SpriteRenderer overlay = new GameObject("Overlay").AddComponent<SpriteRenderer>();
            overlay.sprite = overlaySprites[randomIndex];

            // 부모를 설정하여 위치 고정
            overlay.transform.parent = transform;

            // 위치 무작위 설정 (타일 크기 반영)
            overlay.transform.localPosition = new Vector3(
                Random.Range(-tileSize.x / 2, tileSize.x / 2),
                Random.Range(-tileSize.y / 2, tileSize.y / 2),
                transform.localPosition.z-2.1f // 타일의 Z축과 동일하게 설정
            );

            // Sorting Layer와 Order in Layer를 메인 스프라이트와 동일하게 설정
            overlay.sortingLayerID = mainSpriteRenderer.sortingLayerID;
            overlay.sortingOrder = mainSpriteRenderer.sortingOrder;

            // 크기와 회전 무작위 설정
            overlay.transform.localScale = Vector3.one * Random.Range(randomScaleRange.x, randomScaleRange.y);
            overlay.transform.rotation = Quaternion.Euler(0, 0, Random.Range(randomRotationRange.x, randomRotationRange.y));

            // 투명도 설정
            overlay.color = new Color(1f, 1f, 1f, overlayOpacity);
        }
    }
}
