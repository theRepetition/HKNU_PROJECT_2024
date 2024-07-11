using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform; // RectTransform ������Ʈ ����
    private CanvasGroup canvasGroup; // CanvasGroup ������Ʈ ����
    private Transform originalParent; // �巡�� ���� �� ������Ʈ�� �θ� ����
    private GameObject dragIcon; // �巡�� ���� �������� ���Ͻ� ������Ʈ

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // �巡�� ���� �� �θ� ����

        // �巡�� ������ ����
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(transform.root, false);
        dragIcon.transform.SetAsLastSibling();

        var dragImage = dragIcon.AddComponent<Image>();
        dragImage.sprite = GetComponent<Image>().sprite;
        dragImage.SetNativeSize();

        var dragIconCanvasGroup = dragIcon.AddComponent<CanvasGroup>();
        dragIconCanvasGroup.blocksRaycasts = false;

        canvasGroup.alpha = 0.6f; // ���� ������ ���� ����
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� �������� ��ġ ������Ʈ
        dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragIcon); // �巡�� ������ ����
        canvasGroup.alpha = 1.0f; // ���� ������ ���� ����

        // �������� ���ư� �� ���� �θ�� �ǵ�����
        transform.SetParent(originalParent);
    }
}
