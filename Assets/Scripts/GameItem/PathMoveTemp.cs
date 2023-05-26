using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityGame.App.Manager;

namespace UnityGame.App.Actor
{
    public class PathMoveTemp : IGameItem
    {
        [SerializeField]
        private Rigidbody m_Rigidbody = default;
        [SerializeField]
        private bool m_IsSmoothMove = true;
        private Vector3[] m_Path;
        private int m_PathIndex = 0;
        private bool m_IsMove = false;
        private Vector3 m_LastPosition;
        private Vector3 m_CurrentPosition;
        private float m_Speed => GameMediator.m_GameSetting.Player.MoveSpeed;
        private CharaState m_State;
        private UnityAction m_FixedUpdate;

        public void StartMove(Vector3[] path)
        {
            if (!m_IsMove)
            {
                m_Path = path;
                m_PathIndex = 0;
                m_Rigidbody.position = m_Path[0];
                m_LastPosition = m_CurrentPosition = m_Path[0];
                ChangeState(CharaState.Run);
            }
        }

        private void ChangeState(CharaState state)
        {
            m_State = state;
            switch (m_State)
            {
                case CharaState.Stand:
                    m_FixedUpdate = StandFiexdUpdate;
                    break;
                case CharaState.Run:
                    m_FixedUpdate = RunFixedUpdate;
                    break;
            }
        }

        public override void SystemFixedUpdate()
        {
            m_FixedUpdate?.Invoke();
        }

        public void StandFiexdUpdate()
        { }

        public void RunFixedUpdate()
        {
            if (m_Path != null && m_Path.Length > 0 && m_PathIndex < m_Path.Length)
            {
                Vector3 newPos = GetNextPositionFixed(m_Rigidbody.position, m_Speed, true);
                Vector3 dir = m_Path[m_PathIndex] - newPos;
                m_Rigidbody.MovePosition(newPos);
                m_LastPosition = m_CurrentPosition;
                m_CurrentPosition = newPos;
            }
            else
            {
                ChangeState(CharaState.Stand);
            }
        }

        public Vector3 GetNextPositionFixed(Vector3 currentPos, float speed, bool isFixed = false)
        {
            Vector3 newPos = Vector3.zero;
            Vector3 nextPoint = m_Path[m_PathIndex];
            nextPoint.y = currentPos.y;
            float time = isFixed ? Time.fixedDeltaTime : Time.deltaTime;
            float dis = speed * time;
            float disNextPos = (nextPoint - currentPos).magnitude;
            if (m_IsSmoothMove)
            {
                while (disNextPos < dis && m_PathIndex < m_Path.Length - 1)
                {
                    NextIndex();
                    dis -= disNextPos;
                    currentPos = nextPoint;
                    nextPoint = m_Path[m_PathIndex];
                    nextPoint.y = currentPos.y;
                    disNextPos = (nextPoint - currentPos).magnitude;
                }
            }
            else
            {
                dis = disNextPos;
                NextIndex();
            }
            newPos = Vector3.Lerp(currentPos, nextPoint, dis / disNextPos);
            newPos.y = currentPos.y;
            return newPos;
        }

        private void NextIndex()
        {
            m_PathIndex++;
            m_PathIndex = Mathf.Min(m_Path.Length - 1, m_PathIndex);
        }
    }
}