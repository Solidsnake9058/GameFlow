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
        private bool m_IsStageLoaded = false;
        private bool m_IsStageLoadSuccess = false;
        private bool m_IsAddressablesInit = false;

        public bool GetIsStageLoaded()
        {
            return m_IsStageLoaded;
        }

        public bool GetIsStageLoadSuccess()
        {
            return m_IsStageLoadSuccess;
        }

        public bool GetIsAddressablesInit()
        {
            return m_IsAddressablesInit;
        }

        public override void Initialize(GameManager gameManager)
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
                if (!isGetStage && stageDatas[i].gameObject.activeInHierarchy)
                {
                    m_CurrentStageData = stageDatas[i];
                    isGetStage = true;
                }
                else
                {
                    stageDatas[i].gameObject.SetActive(false);
                }
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
        public bool LoadStage(ref int stageNo, ref int failStage)
        {
            int reqStageNo = stageNo;
            if (!m_StageSettings.m_StageDatas.Any(x => x.m_StageNo == reqStageNo))
                reqStageNo = m_StageSettings.m_StageDatas.First().m_StageNo;
            var stageData = m_StageSettings.m_StageDatas.First(x => x.m_StageNo == reqStageNo);
            //game.StageNo = stageData.StageNo;
            stageNo = reqStageNo = stageData.m_StageNo;
            int stageIndex = m_StageSettings.m_StageDatas.IndexOf(stageData);
            //game.StageDisplayNo = save.Game.StageDisplayNo;

            //m_CurrentStageData = m_StageSettings.GetStageData(stageNo);
            //m_CurrentStageData.Initialize(m_GameManager);

            var order = m_StageSettings.m_StageDatas[stageIndex];
            var prefab = LoadStage(order);
            m_CurrentStageData = Instantiate(prefab);
            if (m_CurrentStageData != null)
            {
                m_CurrentStageData.Initialize(m_GameManager);
                if (order.m_FailToStage > 0 && order.m_FailToStage <= m_StageSettings.m_StageDatas.Count)
                {
                    failStage = m_StageSettings.m_StageDatas[order.m_FailToStage - 1].m_StageNo;
                }
                else
                {
                    failStage = order.m_StageNo;
                }
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

        //public void LoadStageAsync(bool isBonus, ref int stageIndex, ref int displayIndex)
        //{
        //    int displayStage = SaveManager.GetDisplayStage();

            //if (!isBonus)
            //{
            //    int currentStage = SaveManager.GetCurrentStage();
            //    if (!m_StageSettings.Order.Any(x => x.StageNo == currentStage))
            //        currentStage = m_StageSettings.Order.First().StageNo;
            //    var stageData = m_StageSettings.Order.First(x => x.StageNo == currentStage);
            //    stageIndex = stageData.StageNo;
            //    displayIndex = currentStage;

            //    var order = m_StageSettings.Order[stageIndex];
            //    var prefab = m_StageSettings.LoadStage(order, m_CurrentStageData);
            //    Instantiate(prefab);
            //}
            //else
            //{
            //    int currentStage = SaveManager.GetBonusStageIndex();
            //    if (!m_StageSettings.OrderBonus.Any(x => x.StageNo == currentStage))
            //        currentStage = m_StageSettings.OrderBonus.First().StageNo;
            //    var bonusData = m_StageSettings.OrderBonus.First(x => x.StageNo == currentStage);
            //    stageIndex = bonusData.StageNo;
            //    displayIndex = displayIndex / 3;

            //    var order = m_StageSettings.OrderBonus[stageIndex];
            //    var prefab = m_StageSettings.LoadBonus(order, m_CurrentStageData);
            //    var stage = Instantiate(prefab);
            //    //stage.tag = Tag.Game.StageRoot;
            //}
            //string stageKey = $"Stage/Stage{stageIndex:000}.prefab";
            //if (!AddressableStageExists(stageKey))
            //{
            //    Debug.Log($"Stage key \"{stageKey}\" no exist!");
            //    stageIndex = 1;
            //    stageKey = $"Stage/Stage{stageIndex:000}.prefab";
            //}
            //Debug.Log($"Load stage:{stageKey}");
            //m_IsStageLoaded = false;
            //m_IsStageLoadSuccess = false;
            //Addressables.InstantiateAsync(stageKey, Vector3.zero, Quaternion.identity).Completed += OnStageLoaded;
        //}

        //private void OnStageLoaded(AsyncOperationHandle<GameObject> asyncOperationHandle)
        //{
        //    Debug.Log($"OnStageLoaded");
        //    m_IsStageLoaded = true;
        //    m_IsStageLoadSuccess = asyncOperationHandle.IsValid();
        //    m_CurrentStageData = asyncOperationHandle.Result.GetComponent<StageData>();
        //}

        public void ReleaseStatge()
        {
            //Addressables.ReleaseInstance(m_CurrentStageData.gameObject);
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