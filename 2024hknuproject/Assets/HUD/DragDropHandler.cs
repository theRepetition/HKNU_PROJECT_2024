using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private GameObject dragIcon;

    public Sprite dragIconSprite; // 드래그할 때 사용할 스프라이트를 설정하기 위한 변수

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // DragIcon 생성
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(transform.root, false);
        dragIcon.transform.SetAsLastSibling();

        var dragImage = dragIcon.AddComponent<Image>();

        // 드래그할 때 사용할 아이콘 스프라이트를 설정
        if (dragIconSprite != null)
        {
            dragImage.sprite = dragIconSprite;
        }
        else
        {
            dragImage.sprite = GetComponent<Image>().sprite; // dragIconSprite가 없을 경우 원래 스프라이트 사용
        }

        dragImage.SetNativeSize();

        var dragIconCanvasGroup = dragIcon.AddComponent<CanvasGroup>();
        dragIconCanvasGroup.blocksRaycasts = false;

        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragIcon);
        canvasGroup.alpha = 1.0f;
        transform.SetParent(originalParent);
    }
}
