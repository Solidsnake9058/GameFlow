using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchAreaUI : IGameItem, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Camera m_MainCamera = default;
    private Vector2? m_StartPoint = null;

    private bool _IsPointDown = false;
    //public bool m_IsPointDown { get { return _IsPointDown; } }
    public bool GetIsPointDown()
    {
        return _IsPointDown;
    }

    public bool modeToggle = false;

    public float addDistance = 3f;

    private Action m_PointerDownEvent;
    private Action m_PointerUpEvent;

    internal void SetPointerEvent(Action pointerDownEvent, Action pointerUpEvent)
    {
        m_PointerDownEvent = pointerDownEvent;
        m_PointerUpEvent = pointerUpEvent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _IsPointDown = true;
        //m_GameManager.StartGame();
        m_PointerDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _IsPointDown = false;
        m_PointerUpEvent?.Invoke();
    }

    public override void SystemUpdate()
    {
        if (!m_GameManager.GetIsPlaying())
        {
            return;
        }
    }
}