using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.App.Game.Actor
{
    using UnityEngine.Events;
    using UnityEngine.InputSystem;
    using App.Manager;
    using TMPro;

    public class PlayerFowardTemp : IGameItem
    {
        [SerializeField]
        private Rigidbody m_Rigidbody;
        private Animator m_PlayerAnimator;
        [SerializeField]
        private Transform m_TextGroup;
        [SerializeField]
        private Rigidbody m_Follower;
        public Rigidbody Follower => m_Follower;
        [SerializeField]
        private Camera m_Camera;

        private float m_Speed => GameMediator.m_GameSetting.Player.MoveSpeed;
        private float m_SideMoveSpeed => GameMediator.m_GameSetting.Player.SideMoveSpeed;
        private float m_SideMoveRange => GameMediator.m_GameSetting.Player.SideMoveRange;

        //private Vector2 m_BaseRes = new Vector2(640, 1136);
        //private float m_TouchValueTrans = 1;

        private UnityAction m_Update;
        private UnityAction m_FixedUpdate;

        private void Awake()
        {
            //float trans = Mathf.Min(Screen.height / m_BaseRes.y, Screen.width / m_BaseRes.x);
            //m_TouchValueTrans = Mathf.Max(trans, 1);
            m_PlayerAnimator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            GameMediator.m_InputManager.SetStartTouceAction(TouchPressStarted);
        }

        private void Update()
        {
            SystemUpdate();
        }

        private void FixedUpdate()
        {
            SystemFixedUpdate();
        }

        private void LateUpdate()
        {
            m_TextGroup.forward = m_Camera.transform.forward;
        }

        Vector2 m_InputVector = Vector2.zero;
        public override void SystemUpdate()
        {
            m_Update?.Invoke();
        }

        private void RunUpdate()
        {
            m_InputVector = GameMediator.m_InputManager.m_TouchDeltaFix;
        }

        public override void SystemFixedUpdate()
        {
            m_FixedUpdate?.Invoke();
        }

        private void RunFixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            Vector3 pos = m_Rigidbody.position;
            //pos.z += m_Speed * deltaTime;

            if (m_InputVector.x != 0)
            {
                float sideDir = m_InputVector.x;
                float xValue = pos.x;
                //xValue += sideDir * m_SideMoveSpeed * deltaTime;
                //xValue = Mathf.Clamp(xValue, -m_SideMoveRange, m_SideMoveRange);
                pos.x = xValue;
            }
            m_Rigidbody.MovePosition(pos);
            m_Follower.MovePosition(m_Rigidbody.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (other.CompareTag("Goal"))
            //{
            //    m_Update = null;
            //    m_FixedUpdate = null;
            //    Collider goal = other.GetComponent<Collider>();
            //    goal.enabled = false;
            //    StartCoroutine(MoveToGoal(goal.transform.position));
            //}
        }

        private void OnTriggerExit(Collider other)
        {
        }

        private IEnumerator MoveToGoal(Vector3 pos)
        {
            //bool move = true;
            //while (move)
            //{
            //    Vector3 dir = pos - transform.position;
            //    float moveDis = m_Speed * Time.deltaTime;
            //    Vector3 newPos = transform.position + dir.normalized * moveDis;
            //    if (dir.magnitude <= moveDis)
            //    {
            //        newPos = pos;
            //        move = false;
            //    }
            //    transform.position = newPos;
            //    m_Follower.transform.position = new Vector3(0, 0, newPos.z);
            yield return null;
            //}
            //StartCoroutine(FaceToCamera());
        }

        private IEnumerator FaceToCamera()
        {
            //m_PlayerAnimator.SetTrigger("Goal");

            //bool trun = true;
            //float angelCur = transform.rotation.eulerAngles.y;
            //while (trun)
            //{
            //    Vector3 dir = m_Camera.transform.position - transform.position;
            //    float angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
            //    if (angle < 0)
            //    {
            //        angle += 360;
            //    }
            //    angelCur += GameMediator.m_GameManager.PlayerData.GoalRotateSpeed * Time.deltaTime;
            //    if (angelCur >= angle)
            //    {
            //        angelCur = angle;
            //        trun = false;
            //    }
            //    transform.rotation = Quaternion.Euler(0, angelCur, 0);
                yield return null;
            //}
            //GameMediator.m_GameManager.GameClear();
        }

        private void TouchPressStarted()
        {
            Debug.LogWarning($"TouchScreenStarted");
            if (GameMediator.m_GameManager.StartGame())
            {
                m_PlayerAnimator.SetTrigger("Run");
                m_Update = RunUpdate;
                m_FixedUpdate = RunFixedUpdate;
            }
        }
    }
}