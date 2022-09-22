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
        public static GameManager m_Instance { private set; get; }

        //private int m_CurrentLevel;
        private int m_ReplayStage;
        public bool m_IsTestSceneStage;

        //[SerializeField]
        //private CameraController m_CameraController = default;
        //[SerializeField]
        //private GameSettings m_GameSettings = default;
        [SerializeField]
        private StageManager _StageManager = default;
        public StageManager m_StageManager => _StageManager;
        [SerializeField]
        private GameUIManager m_GameUIManager = default;

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

        //public bool GetIsPointDown()
        //{
        //    return m_GameUIManager.GetIsPointDown();
        //}

        [Header("Tenjin")]
        [SerializeField]
        private List<int> m_TenjinClearEventStages = default;

        [SerializeField]
        private int m_StartCalShowIntersititial = 2;
        private bool m_IsCalShowIntersititial;

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

        public void Initialize()
        {
            //m_CameraController.Initialize(m_Instance);
            m_StageManager.Initialize(m_Instance);

            m_GameUIManager.Initialize(m_Instance);

            //m_IsLevelClear = true;
            //m_CurrentStage = 1;
            GameSetting();
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

        private void ManagerGameSetting()
        {
            if (!m_IsTestSceneStage)
            {
                m_CurrentStage = SaveManager.CurrentStageNo;
                m_DisplayStage = SaveManager.DisplayStage;

#if UNITY_EDITOR
                if (EditorPrefs.HasKey("DebugStageNo") && EditorPrefs.GetInt("DebugStageNo") > 0)
                {
                    m_CurrentStage = EditorPrefs.GetInt("DebugStageNo");
                    EditorPrefs.SetInt("DebugStageNo", 0);
                }
#endif

                m_IsCalShowIntersititial = m_DisplayStage >= m_StartCalShowIntersititial;

                _StageManager.LoadStage(ref m_CurrentStage, ref m_ReplayStage);
                m_GameUIManager.FadeInStand();
                ChangeState(GameState.Main);
            }
            else
            {
                _StageManager.GetSceneStage();
                StageData stageData = _StageManager.StageDataCur;
                if (stageData == null)
                {
                    return;
                }
                stageData.Initialize(m_Instance);
                SetStageValue(stageData);
                m_GameUIManager.GameSetting();
                ChangeState(GameState.Main);
            }
            m_GameUIManager.FadeIn();
        }

        private void SetStageValue(StageData stageData)
        {
            m_GameUIManager.SetStageTitle(m_DisplayStage);
        }

        public override void GameSetting()
        {
            ManagerGameSetting();

            //m_GameUIManager.GameSetting();
        }

        void Start()
        {
            //IronSourceManager.ShowBanner();
            Initialize();
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
                    m_GameUIManager.FadeOut();
                    m_Update = FadeOutUpdate;
                    break;
            }
            m_GameUIManager.ChangeState(state);
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
            if (!m_GameUIManager.IsFadeBlack())
            {
                ChangeState(GameState.Play);
            }
            //m_GameUIManager.SetFever(m_FishManager.m_FeverOnRate);
        }

        private void PlayUpdate()
        {
            //m_StageManager.SystemUpdate();
            m_GameUIManager.SystemUpdate();
        }

        private void GameClearUpdate()
        {
            //Debug.Log($"CheckClearEffect:{CheckClearEffect()}");
        }

        private void GameOverAnimationUpdate()
        {
        }

        private void FadeOutUpdate()
        {
            if (m_GameUIManager.IsFadeBlack())
            {
                ReloadGameScecne();
            }
        }

        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        private void GameClearAnimationUpdate()
        {
            m_GameUIManager.SystemUpdate();
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
                ChangeState(GameState.GameOver);
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
#if USE_GAMEANLYTICS
                GameAnalyticsManager.SendEventOnStageFail(m_CurrentStage);
#endif
#endif

                //Invoke("ShowGameoverUI", 3.8f);
                //Invoke("ReplayLevel", 3.8f);
                //ReplayLevel();
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
                SaveManager.SetCurrentStageNo(m_CurrentStage + 1);
                SaveManager.CreaseDisplayStage();
                SaveManager.SetBonusStageCountNext();
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
            //Debug.Log($"NextLevel");
            //Debug.Log($"CurrentStage:{SaveManager.GetCurrentStage()}");
            //int maxIndex = m_StageManager.GetMaxStageIndex();
            int nextIndex = m_CurrentStage + 1; //SaveManager.GetIsAllStageClear(maxIndex) ? 1 : m_CurrentStage + 1;//RNGCryptoServiceProviderExtensions.Next(1, maxIndex + 1) : m_CurrentStage + 1;
            PlayerPrefs.SetInt("CurrentLevel", nextIndex);
            ChangeState(GameState.FadeOut);
            //ReloadGameScecne();
        }

        private void ReloadGameScecne()
        {
            m_StageManager.ReleaseStatge();
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
            m_GameUIManager.SetStageBar(rate);
        }
#endregion

#region Get parameter
        public bool GetIsLevelClear()
        {
            return m_IsLevelClear;
        }

        public int GetDisplayLevel()
        {
            return m_DisplayLevel;
        }

#endregion

    }
}