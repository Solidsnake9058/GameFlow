using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityGame.App.Manager;

namespace UnityGame.App.Actor
{
    public class PlayerDirectionMoveTemp : IGameItem
    {
        [SerializeField]
        private Rigidbody m_Rigidbody = default;
        [SerializeField]
        private Animator m_Animator = default;
        private float m_Speed => GameMediator.m_GameSetting.Player.MoveSpeed;
        [SerializeField]
        private float m_AngleRange = 5f;
        [SerializeField]
        private float m_PushingAngleRange = 45f;
        [SerializeField]
        private float m_MinJSRange = 0.1f;
        private float m_MinJSRangeSqr;
        [SerializeField]
        private float m_RunCheckTime = 0.1f;
        private float m_RunCheckTimeCur;
        [SerializeField]
        private float m_TurnCheckRange = 10f;

        [SerializeField]
        private Transform[] m_OnFloorCheckPos = default;
        //[SerializeField]
        //private OnFloorChecker m_OnFloorChecker = default;
        private Vector3 m_Direction;
        private Vector3 m_NewDirection;
        private Vector3 m_NewPosition;
        private float m_AngleCur;
        private bool m_Turning = false;

        private Vector3 m_UpDir = Vector3.forward;

        private int m_FloorLayer = 0;

        private CharaState m_State;
        private UnityAction m_Update;
        private UnityAction m_FixedUpdate;
        private TouchType m_TouchType = TouchType.StartDelta;
        private UnityAction m_RefreshTouch;

        private void Start()
        {
            GameMediator.m_InputManager.SetStartTouceAction(TouchPressStarted);
            GameSetting();
        }

        private void Update()
        {
            SystemUpdate();
        }

        private void FixedUpdate()
        {
            SystemFixedUpdate();
        }

        public override void SystemUpdate()
        {
            m_Update?.Invoke();
        }

        public override void SystemFixedUpdate()
        {
            m_FixedUpdate?.Invoke();
        }

        public override void GameSetting()
        {
            m_MinJSRangeSqr = m_MinJSRange * m_MinJSRange;
            m_FloorLayer = LayerMask.GetMask(new string[] { "Floor" });
            switch (m_TouchType)
            {
                case TouchType.StartDelta:
                    m_RefreshTouch = RefreshTouchStart;
                    break;
                case TouchType.FrameDelta:
                    m_RefreshTouch = RefreshTouchFrame;
                    break;
            }
        }

        private void RefreshTouchStart()
        {
            m_Direction.x = GameMediator.m_InputManager.m_TouchStartDeltaFix.x;
            m_Direction.z = GameMediator.m_InputManager.m_TouchStartDeltaFix.y;
        }

        private void RefreshTouchFrame()
        {
            m_Direction.x = GameMediator.m_InputManager.m_TouchDeltaFix.x;
            m_Direction.z = GameMediator.m_InputManager.m_TouchDeltaFix.y;
        }

        public void RunUpdate()
        {
            m_RefreshTouch();
        }

        private void ChangeState(CharaState state)
        {
            m_State = state;
            switch (m_State)
            {
                case CharaState.Stand:
                    m_Animator.SetBool("Run", false);
                    m_FixedUpdate = StandFiexdUpdate;
                    break;
                case CharaState.Run:
                    m_Animator.SetBool("Run", true);
                    m_FixedUpdate = RunFixedUpdate;
                    break;
                case CharaState.RunCheck:
                    m_RunCheckTimeCur = 0;
                    m_FixedUpdate = RunCheckFiexdUpdate;
                    break;
            }
        }

        private void StandFiexdUpdate()
        {
            if (CheckDirValue())
            {
                ChangeState(CharaState.Run);
            }
        }

        private void RunFixedUpdate()
        {
            if (m_Turning || CheckDirValue())
            {
                CheckTurn();
                m_NewPosition = m_Rigidbody.position + m_Direction.normalized * m_Speed * Time.fixedDeltaTime;

                //Vector3 checkPos = m_NewPosition + m_Direction * m_OnFloorCheckOffset;
                //RaycastHit[] hits = Physics.RaycastAll(checkPos + m_RayOffset, Vector3.down, 1, m_FloorLayer);
                //if (OnFloorCheck())
                {
                }
                if (!m_Turning)
                {
                    m_Rigidbody.MovePosition(m_NewPosition);
                    m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_NewDirection));
                }
            }
            else
            {
                ChangeState(CharaState.RunCheck);
            }
        }

        private void RunCheckFiexdUpdate()
        {
            CheckTurn();
            if (m_Turning || CheckDirValue())
            {
                ChangeState(CharaState.Run);
            }
            else
            {
                m_RunCheckTimeCur += Time.fixedDeltaTime;
            }

            if (m_RunCheckTimeCur >= m_RunCheckTime)
            {
                ChangeState(CharaState.Stand);
            }
        }

        private bool CheckDirValue()
        {
            if (m_Direction.sqrMagnitude >= m_MinJSRangeSqr)
            {
                return true;
            }
            return false;
        }

        private void CheckTurn()
        {
            m_NewDirection = Vector3.RotateTowards(transform.forward, m_Direction, 1, 0);

            float angle = Vector3.SignedAngle(m_UpDir, m_Direction, Vector3.up) + 180;
            float angleDiff = Mathf.Abs(angle - m_AngleCur);
            if (!m_Turning && angleDiff > 180f - m_TurnCheckRange && angleDiff < 180f + m_TurnCheckRange)
            {
                m_Animator.SetTrigger("Turn");
                m_Turning = true;
            }
            m_AngleCur = angle;
        }

        private bool OnFloorCheck()
        {
            for (int i = 0; i < m_OnFloorCheckPos.Length; i++)
            {
                RaycastHit[] hits = Physics.RaycastAll(m_OnFloorCheckPos[i].position, Vector3.down, 1, m_FloorLayer);
                if (hits.Length == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void TurnEnd()
        {
            m_Turning = false;
            Vector3 newDir = transform.forward * -1;
            newDir.y = 0;
            m_NewDirection = newDir;
            m_Rigidbody.MoveRotation(Quaternion.LookRotation(newDir));
        }

        private void OnCollisionEnter(Collision collision)
        {
        }

        private void OnCollisionExit(Collision collision)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
        }

        private void TouchPressStarted()
        {
            Debug.LogWarning($"TouchScreenStarted");
            if (GameMediator.m_GameManager.StartGame())
            {
                m_Update = RunUpdate;
                ChangeState(CharaState.Stand);
            }
        }
    }


    public enum MoveDir
    {
        None = -1,
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public enum TouchType
    {
        StartDelta,
        FrameDelta
    }
}