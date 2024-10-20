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

        // 드래그 시작 값을 기준으로 새로운 값 계산
        float newValue = dragStartValue + dragDelta;

        // 값을 0~1 범위로 제한
        newValue = Mathf.Clamp01(newValue);

        // 부드러운 전환을 위해 보간 사용
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
                Debug.Log("빠른 위로 스와이프 감지됨");
            }
            else
            {
                DOTween.To(() => scrollRect.verticalNormalizedPosition, x => scrollRect.verticalNormalizedPosition = x, 1, 2f).SetSpeedBased();
                Debug.Log("빠른 아래로 스와이프 감지됨");
            }
        }
        isDragging = false;
    }
}
