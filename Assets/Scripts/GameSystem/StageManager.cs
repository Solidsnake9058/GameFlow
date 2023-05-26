using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGame.Data;

#if USE_ADDRESABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
#endif

namespace UnityGame.App.Manager
{
    using UnityGame.App.Game.Res;
    using System.Linq;

    public class StageManager : IGameItem
    {
        [SerializeField]
        private StageSettings m_StageSettings = default;

        private StageData m_CurrentStageData;
        public bool m_IsAddressablesInit { get; private set; }

        public override void Initialize()
        {
#if USE_ADDRESABLE
            //Addressables.InitializeAsync().Completed += (op) =>
            //{
            //    m_IsAddressablesInit = true;
            //    string key;
            //    key = "Stage/Stage001.prefab";
            //    Debug.Log("Resource exists for " + key + " = " + AddressableStageExists(key));
            //};
#endif
        }

        public void GetSceneStage()
        {
            StageData[] stageDatas = FindObjectsOfType<StageData>();
            bool isGetStage = false;
            for (int i = 0; i < stageDatas.Length; i++)
            {
#if UNITY_EDITOR
                if (!isGetStage && stageDatas[i].gameObject.activeInHierarchy)
                {
                    m_CurrentStageData = stageDatas[i];
                    isGetStage = true;
                }
                else
                {
                    stageDatas[i].gameObject.SetActive(false);
                }
#else
                Destroy(stageDatas[i].gameObject);
#endif
            }
        }

        //public bool CheckStageClear()
        //{
        //    m_GameManager.SetTargetLeft(GetLeftTargetCount());
        //    return m_CurrentStageData.CheckStageClear();
        //}

        //private int GetLeftTargetCount()
        //{
        //    return m_CurrentStageData != null ? m_CurrentStageData.GetLeftTargetCount() : 99;
        //}

        /// <summary>
        /// Loads the stage and keep stage index between 1 and max index.
        /// </summary>
        /// <returns><c>true</c>, if stage was loaded, <c>false</c> level is not exists!.</returns>
        /// <param name="level">Level.</param>
        public virtual bool LoadStage(ref int stageNo, ref int failStage)
        {
            int reqStageNo = stageNo;
            if (m_StageSettings == null || m_StageSettings.m_StageDatas.Count == 0)
            {
                return false;
            }
            if (!m_StageSettings.m_StageDatas.Any(x => x.m_StageNo == reqStageNo))
            {
                reqStageNo = m_StageSettings.m_StageDatas.First().m_StageNo;
            }
            var stageData = m_StageSettings.m_StageDatas.First(x => x.m_StageNo == reqStageNo);
            //game.StageNo = stageData.StageNo;
            stageNo = reqStageNo = failStage = stageData.m_StageNo;
            int stageIndex = m_StageSettings.m_StageDatas.IndexOf(stageData);
            //game.StageDisplayNo = save.Game.StageDisplayNo;

            //m_CurrentStageData = m_StageSettings.GetStageData(stageNo);
            //m_CurrentStageData.Initialize(m_GameManager);

            var order = m_StageSettings.m_StageDatas[stageIndex];
            var prefab = LoadStage(order);
            m_CurrentStageData = Instantiate(prefab);
            return SetStageDate(m_CurrentStageData);
        }

        protected bool SetStageDate(StageData stageData)
        {
            if (stageData != null)
            {
                m_CurrentStageData = stageData;
                return true;
            }
            return false;
        }

        public StageData LoadStage(StageSettings.StageSetting data)
        {
            return (StageData)Loader.Load($"{data.m_Name}");
        }

        public StageData LoadBonus(StageSettings.StageSetting data)
        {
            return (StageData)Loader.Load($"{data.m_Name}");
        }

        //public bool AddressableStageExists(object key)
        //{
        //    foreach (var l in Addressables.ResourceLocators)
        //    {
        //        IList<IResourceLocation> locs;
        //        if (l.Locate(key, typeof(GameObject), out locs))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public StageData StageDataCur => m_CurrentStageData;

        public override void SystemUpdate()
        {
            m_CurrentStageData.SystemUpdate();
        }

        public override void SystemFixedUpdate()
        {
            m_CurrentStageData.SystemFixedUpdate();
        }

        //public int GetMaxStageIndex()
        //{
        //    return m_StageSettings.GetMaxStageIndex();
        //}

        //public StageData GetMaxStage()
        //{
        //    return m_StageSettings.GetMaxStage();
        //}
    }
}