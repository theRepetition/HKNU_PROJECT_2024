using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform; // RectTransform 컴포넌트 참조
    private CanvasGroup canvasGroup; // CanvasGroup 컴포넌트 참조
    private Transform originalParent; // 드래그 시작 시 오브젝트의 부모 저장
    private GameObject dragIcon; // 드래그 중인 아이템의 프록시 오브젝트

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // 드래그 시작 시 부모 저장

        // 드래그 아이콘 생성
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(transform.root, false);
        dragIcon.transform.SetAsLastSibling();

        var dragImage = dragIcon.AddComponent<Image>();
        dragImage.sprite = GetComponent<Image>().sprite;
        dragImage.SetNativeSize();

        var dragIconCanvasGroup = dragIcon.AddComponent<CanvasGroup>();
        dragIconCanvasGroup.blocksRaycasts = false;

        canvasGroup.alpha = 0.6f; // 원래 슬롯의 투명도 조정
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 아이콘의 위치 업데이트
        dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragIcon); // 드래그 아이콘 삭제
        canvasGroup.alpha = 1.0f; // 원래 슬롯의 투명도 복원

        // 슬롯으로 돌아갈 때 원래 부모로 되돌리기
        transform.SetParent(originalParent);
    }
}
