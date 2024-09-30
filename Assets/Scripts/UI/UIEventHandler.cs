using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action OnClickHandler = null;
    public Action OnPressedHandler = null;
    public Action OnPointerDownHandler = null;
    public Action OnPointerUpHandler = null;

    bool _pressed = false;

    private void Update()
    {
        if (_pressed)
            OnPressedHandler?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        OnPointerDownHandler?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        OnPointerUpHandler?.Invoke();
    }
}
