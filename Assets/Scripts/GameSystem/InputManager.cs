using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityGame.App.Actor;
using static UnityEngine.EventSystems.ExecuteEvents;

namespace UnityGame.App.Manager
{
    public class InputManager : IGameItem
    {
        [SerializeField]
        private EventSystem m_EventSystem;
        [SerializeField]
        private InputAction m_PressAction;
        [SerializeField]
        private InputAction m_ScreenPos;
        private Vector3 m_ScreenPosCur;
        [SerializeField]
        private Camera _MainCamera;
        private Camera m_MainCamera
        {
            get
            {
                if (_MainCamera == null)
                {
                    _MainCamera = Camera.main;
                }
                return _MainCamera;
            }
        }

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

        //public void SetPointerEvent(Action pointerDownEvent, Action pointerUpEvent)
        //{
        //    m_PointerDownEvent = pointerDownEvent;
        //    m_PointerUpEvent = pointerUpEvent;
        //}

        private void Awake()
        {
            if (_MainCamera == null)
            {
                _MainCamera = Camera.main;
            }
            //!Get touch position first
            m_ScreenPos.Enable();
            m_ScreenPos.Reset();
            m_ScreenPos.performed += GetScreenPress;
            m_PressAction.Enable();
            m_PressAction.Reset();
            m_PressAction.performed += ObjectPress;

            Input.multiTouchEnabled = false;
            SetScreenRate();
        }

        private void OnDestroy()
        {
            m_PressAction.Disable();
            m_ScreenPos.Disable();
            m_ScreenPos.performed -= GetScreenPress;
            m_PressAction.performed -= ObjectPress;
        }

        public void ObjectPress(InputAction.CallbackContext context)
        {
            Debug.Log("InputManager ObjectPress");
            StartCoroutine(ClickProcess());
        }

        private IEnumerator ClickProcess()
        {
            yield return null;
            //Check UI block at next frame
            if (m_EventSystem.IsPointerOverGameObject())
            {
                //Check turotial touch area
                var data = new PointerEventData(EventSystem.current);
                var results = new List<RaycastResult>();
                data.position = m_ScreenPosCur;
                data.delta = Vector2.zero;
                data.button = PointerEventData.InputButton.Left;
                EventSystem.current.RaycastAll(data, results);
                bool isTutBlock = false;
                bool isTutTouch = false;
                foreach (var item in results)
                {
                    isTutBlock |= item.gameObject.CompareTag("TutorialBlock");
                    isTutTouch |= item.gameObject.CompareTag("TutorialTouch");
                    if (item.gameObject.CompareTag("TutorialBlock") || item.gameObject.CompareTag("TutorialTouch"))
                    {
                        continue;
                    }
                    if (!isTutBlock || !isTutTouch)
                    {
                        yield break;
                    }
                    GameObject objNext = item.gameObject;
                    if (objNext != null)
                    {
                        GameObject objExe = GetEventHandler<IEventSystemHandler>(objNext);
                        if (objExe != null)
                        {
                            Debug.Log("Execute");
                            Execute(objExe, data, pointerClickHandler);
                            yield break;
                        }
                    }
                }
                if (isTutBlock && isTutTouch)
                {
                    GameEvent.Trigger(TaskEventType.TouchClick, 0, 0);
                }
                if (!(isTutBlock && isTutTouch))
                {
                    Debug.Log("UI over");
                    yield break;
                }
            }
            var clickable = GetClickableItem();
            if (clickable != null)
            {
                Debug.Log("clickable");
                clickable.ClickAction(GetClickWorldPoint());
            }
            else
            {
                Debug.Log("Out of clickable object");
            }
        }

        public void GetScreenPress(InputAction.CallbackContext context)
        {
            Debug.Log("InputManager GetScreenPress");
            m_ScreenPosCur = context.ReadValue<Vector2>();
        }

        public Vector3 GetClickWorldPoint()
        {
            return m_MainCamera.ScreenToWorldPoint(m_ScreenPosCur);
        }

        public ClickableItem GetClickableItem()
        {
            Ray ray = m_MainCamera.ScreenPointToRay(m_ScreenPosCur);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null)
            {
                return hit.transform.GetComponent<ClickableItem>();
            }
            return null;
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

        //public void SetStartTouceAction(Action action)
        //{
        //    m_StartTouchAction = action;
        //}

        //private void StartTouchPressStarted(InputAction.CallbackContext context)
        //{
        //    m_StartTouchAction?.Invoke();
        //    EnableTouch();
        //    m_GameInput.GameTouchStart.Disable();
        //}

        //private void TouchPressStarted(InputAction.CallbackContext context)
        //{
        //    _IsDragging = true;
        //    StartCoroutine(TouchPressing());
        //    m_PointerDownEvent?.Invoke();
        //}

        //private void TouchPressCanceled(InputAction.CallbackContext context)
        //{
        //    StopAllCoroutines();
        //    Reset();
        //    m_PointerUpEvent?.Invoke();
        //}

        //private IEnumerator TouchPressing()
        //{
        //    m_TouchCurrent = m_TouchStart = m_GameInput.GameTouchMain.TouchPosition.ReadValue<Vector2>();
        //    while (true)
        //    {
        //        m_TouchCurrent = m_GameInput.GameTouchMain.TouchPosition.ReadValue<Vector2>();
        //        m_TouchDelta = m_GameInput.GameTouchMain.TouchDelta.ReadValue<Vector2>();
        //        m_TouchStartDelta = m_TouchCurrent - m_TouchStart;
        //        //Debug.Log($"Touch current:({m_TouchCurrent.x},{m_TouchCurrent.y}) delta:({m_TouchDelta.x},{m_TouchDelta.y})");
        //        yield return null;
        //    }
        //}

        //private void Reset()
        //{
        //    _IsDragging = false;
        //    m_TouchStart = m_TouchCurrent = m_TouchDelta = m_TouchStartDelta = Vector2.zero;
        //}

        //private void OnEnable()
        //{
        //}

        //private void OnDisable()
        //{
        //}

        //private void OnApplicationQuit()
        //{
        //}

    }
}