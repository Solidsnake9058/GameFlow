using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnityGame.App.Manager
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class GameManager : IGameItem
    {
        private int m_ReplayStage;

        [SerializeField]
        private List<ParticleSystem> m_ClearParticles = default;
        [SerializeField]
        private float m_ClearDelayTime = 0.5f;
        private float m_ClearDelayTimeCurrent;

        [SerializeField]
        private float m_ClearCameraDelayTime = 2f;
        private float m_ClearCameraDelayTimeCurrent;

        private GameState m_GameState;
        private UnityAction m_Update;
        private UnityAction m_FixedUpdate;
        private UnityAction m_LateUpdate;

        public float m_CheckDelay = 0;
        private float m_CheckDelayCurrent = 0;

        public bool GetIsPlaying()
        {
            return m_GameState == GameState.Play;
        }

        private bool m_IsLevelClear = false;
        private int m_CurrentStage = 0;
        private int m_DisplayLevel = 0;
        private int m_DisplayStage = 0;

        public bool m_IsGameOver = false;

        [Header("Tenjin")]
        [SerializeField]
        private List<int> m_TenjinClearEventStages = default;

        [SerializeField]
        private int m_StartCalShowIntersititial = 2;
        private bool m_IsCalShowIntersititial;

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

        public void Init()
        {
            //m_CameraController.Initialize(m_Instance);
            //GameMediator.m_StageManager.Initialize(m_Instance);

            //GameMediator.m_GameUIManager.Initialize(m_Instance);

            //m_IsLevelClear = true;
            //m_CurrentStage = 1;
            Setting();
        }

        public void StartGame()
        {
            Debug.Log("StartGame");
            if (m_GameState == GameState.Main)
            {
                Debug.Log("Play");
                ChangeState(GameState.Play);
            }
        }

        private void Setting()
        {
            GameMediator.m_GameUIManager.GameSetting();

            bool isLoadTest = false;
            m_CurrentStage = SaveManager.CurrentStageNo;
            m_DisplayStage = SaveManager.DisplayStage;

#if UNITY_EDITOR
            if (EditorPrefs.HasKey("DebugStageNo") && EditorPrefs.GetInt("DebugStageNo") > 0)
            {
                m_CurrentStage = EditorPrefs.GetInt("DebugStageNo");
                m_DisplayStage = m_CurrentStage;
                EditorPrefs.SetInt("DebugStageNo", 0);
                isLoadTest = true;
            }
#endif

            GameMediator.m_StageManager.GetSceneStage();
            if (isLoadTest || GameMediator.m_StageManager.StageDataCur == null)
            {
                m_IsCalShowIntersititial = m_DisplayStage >= m_StartCalShowIntersititial;

                GameMediator.m_StageManager.LoadStage(ref m_CurrentStage, ref m_ReplayStage);
                GameMediator.m_GameUIManager.FadeInStand();
            }
            ChangeState(GameState.Main);
            SetStageValue(GameMediator.m_StageManager.StageDataCur);
            GameMediator.m_GameUIManager.FadeIn();
        }

        private void SetStageValue(StageData stageData)
        {
            GameMediator.m_GameUIManager.SetStageTitle(m_DisplayStage);
        }

        void Start()
        {
            //IronSourceManager.ShowBanner();
            Init();
        }

        void Update()
        {
            m_Update?.Invoke();
        }

        public void ChangeState(GameState state)
        {
            m_GameState = state;
            switch (state)
            {
                case GameState.Main:
                    m_Update = MainUpdate;
                    break;
                case GameState.Play:
                    m_CheckDelayCurrent = 0;
                    m_Update = PlayUpdate;
                    //IronSourceManager.ShowBanner();
                    break;
                case GameState.GameClearAnimation:
                    m_Update = GameClearAnimationUpdate;
                    break;
                case GameState.GameClear:
                    PlayClearEffect();
                    //IronSourceManager.HideBanner(IronSourceManager.BannerType.Rectangle);
                    //IronSourceManager.ShowBanner();
                    m_Update = GameClearUpdate;
                    break;
                case GameState.GameOverAnimation:
                    //PlayerPrefs.SetInt("MainUI", 1);
                    m_Update = GameOverAnimationUpdate;
                    break;
                case GameState.GameOver:
                    Debug.Log("GameOver");
                    m_Update = null;
                    break;
                case GameState.FadeOut:
                    GameMediator.m_GameUIManager.FadeOut();
                    m_Update = FadeOutUpdate;
                    break;
            }
            GameMediator.m_GameUIManager.ChangeState(state);
        }

        public void CalShowIntersititial()
        {
            if (m_IsCalShowIntersititial)
            {
                //IronSourceManager.ShowInterstitial();
            }
        }

        private void MainUpdate()
        {
            if (!GameMediator.m_GameUIManager.IsFadeBlack())
            {
                ChangeState(GameState.Play);
            }
        }

        private void PlayUpdate()
        {
            //m_StageManager.SystemUpdate();
            GameMediator.m_GameUIManager.SystemUpdate();
        }

        private void GameClearUpdate()
        {
        }

        private void GameOverAnimationUpdate()
        {
        }

        private void FadeOutUpdate()
        {
            if (GameMediator.m_GameUIManager.IsFadeBlack())
            {
                ReloadGameScecne();
            }
        }

        private void GameClearAnimationUpdate()
        {
            GameMediator.m_GameUIManager.SystemUpdate();
            m_ClearDelayTimeCurrent += Time.deltaTime;
            if (m_ClearDelayTimeCurrent >= m_ClearDelayTime)
            {
                ChangeState(GameState.GameClear);
            }
        }

        private void PlayClearEffect()
        {
            for (int i = 0; i < m_ClearParticles.Count; i++)
            {
                m_ClearParticles[i]?.Play();
            }
        }

        private void ShowGameoverUI()
        {
            ChangeState(GameState.GameOver);
        }

        public void ToMain()
        {
            ChangeState(GameState.Main);
        }

        public void GameOver()
        {
            if (m_GameState == GameState.Play)
            {
                SaveManager.SetStageClear(m_CurrentStage, StageAssets.StageState.NotClear);
                SaveManager.Save();
                ChangeState(GameState.GameOver);
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
#if USE_GAMEANLYTICS
                GameAnalyticsManager.SendEventOnStageFail(m_CurrentStage);
#endif
#endif
            }
        }

        public void GameClear()
        {
            if (m_GameState == GameState.Play)
            {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
#if USE_TENJIN
                if (m_TenjinClearEventStages.Contains(m_CurrentStage))
                {
                    TenjinManager.SendEvent($"stage{m_CurrentStage}_complete");
                }
#endif
#if USE_GAMEANLYTICS
                if (!SaveManager.IsStageCleared(m_CurrentStage))
                {
                    GameAnalyticsManager.SendEventOnStageClear(m_CurrentStage);
                }
#endif
#endif

                SaveManager.SetStageClear(m_CurrentStage, StageAssets.StageState.Clear);
                SaveManager.Save();
                ChangeState(GameState.GameClear);
            }
        }

        public void ReplayLevel()
        {
            //Debug.Log($"ReplayLevel");
            SaveManager.SetCurrentStageNo(m_ReplayStage);
            SaveManager.Save();
            ChangeState(GameState.FadeOut);
            //ReloadGameScecne();
        }

        public void ReplayStart()
        {
            ReloadGameScecne();
        }

        public void NextLevel()
        {
            ChangeState(GameState.FadeOut);
            //ReloadGameScecne();
        }

        private void ReloadGameScecne()
        {
            SceneManager.LoadScene("Game");
        }

        #region Public method
        public bool CheckClearEffect()
        {
            bool isStopped = true;
            for (int i = 0; i < m_ClearParticles.Count; i++)
            {
                isStopped &= m_ClearParticles[i].isStopped;
            }
            return isStopped;
        }

        public void SetStageBar(float rate)
        {
            GameMediator.m_GameUIManager.SetStageBar(rate);
        }
        #endregion
    }
}