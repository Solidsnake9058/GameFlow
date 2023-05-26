using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.App.Manager
{
    using UnityGame.Data;

    public class GameMediator : MonoBehaviour
    {
        public static GameMediator m_Instance { private set; get; }

        [SerializeField]
        private GameManager _GameManager = default;
        public static GameManager m_GameManager => m_Instance._GameManager;
        [SerializeField]
        private StageManager _StageManager = default;
        public static StageManager m_StageManager => m_Instance._StageManager;
        [SerializeField]
        private InputManager _InputManager = default;
        public static InputManager m_InputManager => m_Instance._InputManager;
        [SerializeField]
        private GameUIManager _GameUIManager = default;
        public static GameUIManager m_GameUIManager => m_Instance._GameUIManager;


        [SerializeField]
        private GameSetting _GameSetting;
        public static GameSetting m_GameSetting => m_Instance._GameSetting;
        [SerializeField]
        private AppSetting _AppSetting;
        public static AppSetting m_AppSetting => m_Instance._AppSetting;
        [SerializeField]
        private CustomIntegerUnitSetting _CustomIntegerUnitSetting;
        public static CustomIntegerUnitSetting m_CustomIntegerUnitSetting => m_Instance._CustomIntegerUnitSetting;

        private void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}