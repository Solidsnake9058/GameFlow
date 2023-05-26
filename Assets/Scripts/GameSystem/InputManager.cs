using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameInput;

public class InputManager : IGameItem
{
    private GameInput m_GameInput;
    public Vector2 m_TouchStart { get; private set; }
    public Vector2 m_TouchCurrent { get; private set; }
    public Vector2 m_TouchDelta { get; private set; }
    public Vector2 m_TouchDeltaFix => m_TouchDelta * m_TouchRate;
    public Vector2 m_TouchStartDelta { get; private set; }
    public Vector2 m_TouchStartDeltaFix => m_TouchStartDelta * m_TouchRate;

    //iPhone 6 :Resolution 750X1334, desity :2
    private Vector2 m_BaseResolution = new Vector2(750, 1334);
    private float m_BaseDensity = 2;
    private float m_TouchRate = 1;
    private float _ScreenRate = 16f / 9f;
    public float m_ScreenRate { get { return _ScreenRate; } }
    private bool _IsDragging = false;
    public bool m_IsDragging { get { return _IsDragging; } }
    private Action m_PointerDownEvent;
    private Action m_PointerUpEvent;
    private Action m_StartTouchAction;

    public void SetPointerEvent(Action pointerDownEvent, Action pointerUpEvent)
    {
        m_PointerDownEvent = pointerDownEvent;
        m_PointerUpEvent = pointerUpEvent;
    }

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        SetScreenRate();

        m_GameInput = new GameInput();
        m_GameInput.GameTouchStart.Enable();
        m_GameInput.GameTouchStart.TouchPress.started += ctx => StartTouchPressStarted(ctx);

        //m_GameInput.GameTouchMain.Enable();
        //m_GameInput.GameTouchMain.TouchPress.started += ctx => TouchPressStarted(ctx);
        //m_GameInput.GameTouchMain.TouchPress.canceled += ctx => TouchPressCanceled(ctx);
    }

    private void SetScreenRate()
    {
        float screenRate = (float)Screen.height / Screen.width;
        float newHeight = Screen.height;
        float newWidth = Screen.width;

        float touchRateWidth = m_BaseResolution.x / Screen.width;
        float touchRateHeight = m_BaseResolution.y / Screen.height;
        m_TouchRate = Mathf.Min(touchRateWidth, touchRateHeight);
        m_TouchRate = Mathf.Max(m_TouchRate, 1);

        if (screenRate <= _ScreenRate)
        {
            newWidth = (newHeight / 9f) * 16f;
        }

        _ScreenRate = (float)newWidth / newHeight;
    }

    public void SetStartTouceAction(Action action)
    {
        m_StartTouchAction = action;
    }

    public void EnableTouch()
    {
        Debug.Log("EnableTouch");
        m_GameInput.GameTouchMain.Enable();
        m_GameInput.GameTouchMain.TouchPress.started += ctx => TouchPressStarted(ctx);
        m_GameInput.GameTouchMain.TouchPress.canceled += ctx => TouchPressCanceled(ctx);
    }

    public void DisableTouch()
    {
        m_GameInput.GameTouchMain.Disable();
        m_GameInput.GameTouchMain.TouchPress.RemoveAction();
        StopAllCoroutines();
        Reset();
    }

    private void StartTouchPressStarted(InputAction.CallbackContext context)
    {
        m_StartTouchAction?.Invoke();
        EnableTouch();
        m_GameInput.GameTouchStart.Disable();
    }

    private void TouchPressStarted(InputAction.CallbackContext context)
    {
        _IsDragging = true;
        StartCoroutine(TouchPressing());
        m_PointerDownEvent?.Invoke();
    }

    private void TouchPressCanceled(InputAction.CallbackContext context)
    {
        StopAllCoroutines();
        Reset();
        m_PointerUpEvent?.Invoke();
    }

    private IEnumerator TouchPressing()
    {
        m_TouchCurrent = m_TouchStart = m_GameInput.GameTouchMain.TouchPosition.ReadValue<Vector2>();
        while (true)
        {
            m_TouchCurrent = m_GameInput.GameTouchMain.TouchPosition.ReadValue<Vector2>();
            m_TouchDelta = m_GameInput.GameTouchMain.TouchDelta.ReadValue<Vector2>();
            m_TouchStartDelta = m_TouchCurrent - m_TouchStart;
            //Debug.Log($"Touch current:({m_TouchCurrent.x},{m_TouchCurrent.y}) delta:({m_TouchDelta.x},{m_TouchDelta.y})");
            yield return null;
        }
    }

    private void Reset()
    {
        _IsDragging = false;
        m_TouchStart = m_TouchCurrent = m_TouchDelta = m_TouchStartDelta = Vector2.zero;
    }

    private void OnEnable()
    {
        //m_GameInput.Start.Enable();
        //m_GameInput.Start.TouchPress.started += ctx => TouchPressStarted(ctx);

        //m_GameInputActions.Player.Enable();

    }

    private void OnDisable()
    {
        //m_GameInput.Start.Disable();
        m_GameInput.GameTouchMain.Disable();
    }

    private void OnApplicationQuit()
    {
        m_GameInput.Dispose();
        m_GameInput.Disable();
    }

}
