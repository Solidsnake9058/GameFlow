using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityGame.App.Manager
{
    public class GameUIManager : IGameItem
    {
        [SerializeField]
        private MainUI m_MainUI = default;
        [SerializeField]
        private GameUI m_GameUI = default;
        [SerializeField]
        private GameOverUI m_GameOverUI = default;
        [SerializeField]
        private GameClearUI m_GameClearUI = default;
        [SerializeField]
        private FadeUI m_FadeUI = default;

        private UnityAction m_Update;
        //private bool m_IsShowBannerExtraPage = false;

        public override void GameSetting()
        {
            m_GameOverUI.GameSetting();
            m_GameClearUI.GameSetting();
        }

        public override void Initialize()
        {
            base.Initialize();
            m_MainUI.Initialize();
            m_GameUI.Initialize();
            m_GameOverUI.Initialize();
            m_GameClearUI.Initialize();
            m_FadeUI.Initialize();
        }

        public override void Pause()
        {
        }

        public override void Resume()
        {
        }

        //public void GamePointerDown(PointerEventData eventData)
        //{
        //    m_GameUI.GamePointerDown(eventData);
        //}

        public void ChangeState(GameState state)
        {
            //m_IsShowBannerExtraPage = false;
            switch (state)
            {
                case GameState.Main:
                    m_MainUI.ShowUI();
                    m_Update = EmptyUpdate;
                    break;
                case GameState.Play:
                    m_MainUI.HideUI();
                    m_GameUI.SetUIDisplay(true);
                    m_Update = PlayUpdate;
                    break;
                case GameState.GameOver:
                    m_GameOverUI.ShowUI();
                    break;
                case GameState.GameClear:
                    m_GameClearUI.ShowUI();
                    break;
            }
        }

        public void SetStageBar(float rate) => m_GameUI.SetStageBar(rate);

        public void SetStageTitle(int index) => m_GameUI.SetStageTitle(index);

        public void ShowStageSelect()
        {
            //IronSourceManager.HideBanner();
        }

        public override void SystemUpdate() => m_Update?.Invoke();

        private void ShowAD()
        {
            //IronSourceManager.ShowBanner();
            //m_IsShowBannerExtraPage = true;
        }

        #region Update
        private void PlayUpdate()
        {
            m_GameUI.SystemUpdate();
        }

        private void EmptyUpdate()
        {

        }

        #endregion

        #region Fade
        public void FadeInStand() => m_FadeUI.FadeInStand();

        public void FadeIn() => m_FadeUI.FadeIn();

        public void FadeOut() => m_FadeUI.FadeOut();

        public bool IsFadeBlack() => m_FadeUI.m_IsInBlack;
        #endregion
    }
}