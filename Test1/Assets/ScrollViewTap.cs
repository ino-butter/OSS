using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScrollViewTap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public float swipeThreshold = 50f;
    public float timeThreshold = 0.3f;
    private float startTime;

    [SerializeField]
    private ScrollRect scrollRect;
    private Vector2 dragStartPosition;
    private float dragStartValue;
    private bool isDragging = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        dragStartPosition = eventData.position;
        dragStartValue = scrollRect.verticalNormalizedPosition;
        startTime = Time.time;
    }

    private float CalculateInvertedNormalizedValue(float dragDistance, float totalDistance)
    {
        float rawValue = dragDistance / totalDistance;
        return 1 - Mathf.Clamp01(rawValue);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 currentDragPosition = eventData.position;
        float dragDelta = (dragStartPosition.y - currentDragPosition.y) / scrollRect.viewport.rect.height;

        // �巡�� ���� ���� �������� ���ο� �� ���
        float newValue = dragStartValue + dragDelta;

        // ���� 0~1 ������ ����
        newValue = Mathf.Clamp01(newValue);

        // �ε巯�� ��ȯ�� ���� ���� ���
        float smoothValue = Mathf.Lerp(scrollRect.verticalNormalizedPosition, newValue, Time.deltaTime * 10f);

        scrollRect.verticalNormalizedPosition = smoothValue;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 endPos = eventData.position;
        float endTime = Time.time;

        float distance = Mathf.Abs(endPos.y - dragStartPosition.y);
        float duration = endTime - startTime;
        float speed = distance / duration;

        if (speed >= swipeThreshold && duration <= timeThreshold)
        {
            if (endPos.y > dragStartPosition.y)
            {
                DOTween.To(() => scrollRect.verticalNormalizedPosition, x => scrollRect.verticalNormalizedPosition = x, 0, 2f).SetSpeedBased();
                Debug.Log("���� ���� �������� ������");
            }
            else
            {
                DOTween.To(() => scrollRect.verticalNormalizedPosition, x => scrollRect.verticalNormalizedPosition = x, 1, 2f).SetSpeedBased();
                Debug.Log("���� �Ʒ��� �������� ������");
            }
        }
        isDragging = false;
    }
}
