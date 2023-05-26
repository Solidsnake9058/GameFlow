#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.Data;

namespace UnityGame.App.Manager
{
    public class CreatorMediator : GameMediator
    {
        public static CreatorMediator m_InstanceCreator => (CreatorMediator)m_Instance;

        [SerializeField]
        private CreateShapeManager _CreateShapeManager = default;
        public static CreateShapeManager m_CreateShapeManager => m_InstanceCreator._CreateShapeManager;
        [SerializeField]
        private CreatorUIManager _CreatorUIManager = default;
        public static CreatorUIManager m_CreatorUIManager => m_InstanceCreator._CreatorUIManager;

        //[SerializeField]
        //private StageCreatorSetting _StageCreatorSetting;
        //public static StageCreatorSetting m_StageCreatorSetting => m_InstanceCreator._StageCreatorSetting;
        //[SerializeField]
        //private GameSetting _GameSetting;
        //public static GameSetting m_GameSetting => m_Instance._GameSetting;

        //private void Awake()
        //{
        //    if (m_Instance == null)
        //    {
        //        m_Instance = this;
        //    }
        //    else
        //    {
        //        Destroy(gameObject);
        //    }
        //}
    }
}
#endif