using UnityEngine;
using System.Collections;
using UnityGame.App.Manager;

public class DragHandler : IGameSystem
{
    public bool m_IsDragging;

    public Vector2 m_StartTouchPoint;
    public Vector2 m_CurrentTouchPoint;

    public Vector2 m_LastFrameTouchPoint;

    //iPhone 6 :Resolution 750X1334, desity :2
    public Vector2 m_BaseResolution = new Vector2(750, 1334);
    public float m_BaseDensity = 2;

    private TouchAreaUI m_TapArea;

    private float m_ScreenRate = 16f / 9f;

    public bool IsDraggingDown => m_StartTouchPoint.y > m_CurrentTouchPoint.y;

    private float ScreenRate
    {
        get
        {
            return m_ScreenRate;
        }
    }

    public DragHandler(GameManager gameManager) : base(gameManager)
    {
    }

    public override void Initialize()
    {
        Input.multiTouchEnabled = false;
    }

    private void SetScreenRate()
    {
        float screenRate = (float)Screen.height / Screen.width;
        float newHeight = Screen.height;
        float newWidth = Screen.width;

        if (screenRate <= m_ScreenRate)
        {
            newWidth = (newHeight / 9f) * 16f;
        }

        m_ScreenRate = (float)newWidth / newHeight;
    }

    public void SetTapArea(TouchAreaUI tapArea)
    {
        m_TapArea = tapArea;
        m_TapArea.SetPointerEvent(PointerDownEvent, PointerUpEvent);
    }

    public override void SystemUpdate()
    {
        HandleInput();
    }

    void HandleInput()
    {
        //if (m_TapArea.m_IsTaping)
        //{
        //    PointerDownEvent();
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    PointerUpEvent();
        //}

        if (m_IsDragging)
        {
            m_LastFrameTouchPoint = m_CurrentTouchPoint;
            m_CurrentTouchPoint = Input.mousePosition;
        }
        else
        {
            //currentTouchPoint = Utility.Utility.Vector3Zero();
            m_LastFrameTouchPoint = m_CurrentTouchPoint;
        }

    }

    private void PointerUpEvent()
    {
        m_IsDragging = false;
        //startTouchPoint = Vector3Zeroz();
    }

    private void PointerDownEvent()
    {
        m_IsDragging = true;
        m_StartTouchPoint = Input.mousePosition;

        m_CurrentTouchPoint = m_StartTouchPoint;
        m_LastFrameTouchPoint = m_StartTouchPoint;
    }

    public float TouchStartDistance()
    {
        return Vector2.Distance(m_StartTouchPoint, m_CurrentTouchPoint) * ScreenRate;
    }

    public Vector2 TouchDelta()
    {
        return (m_CurrentTouchPoint - m_LastFrameTouchPoint) * ScreenRate;
    }

    public Vector2 TouchStartDirector()
    {
        return (m_CurrentTouchPoint - m_StartTouchPoint) * ScreenRate;
    }

    public void ForceReset()
    {
        m_IsDragging = false;
    }

   

    public override void Pause()
    {
    }

    public override void Resume()
    {
    }
}
