#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame.App.UI
{
    using UnityGame.App.Manager;
    using TMPro;

    public class StageFileUI : IGameUISystem
    {
        [SerializeField]
        private Button m_UpperBtn;
        [SerializeField]
        private Button m_LowerBtn;

        [SerializeField]
        private Button m_CreateNewBtn;
        [SerializeField]
        private Button m_SaveBtn;
        [SerializeField]
        private Button m_LoadBtn;
        [SerializeField]
        private Button m_DeleteBtn;

        [SerializeField]
        private Button m_TestPlayBtn;
        [SerializeField]
        private Button m_StopTestBtn;

        [SerializeField]
        private Image m_StateBG;
        [SerializeField]
        private TextMeshProUGUI m_StateText;
        [SerializeField]
        private TextMeshProUGUI m_TapCountText;

        private void Start()
        {
            m_UpperBtn.onClick.AddListener(() => CreatorMediator.m_CreateShapeManager.StageShiftSort(true));
            m_LowerBtn.onClick.AddListener(() => CreatorMediator.m_CreateShapeManager.StageShiftSort(false));

            m_CreateNewBtn.onClick.AddListener(CreatorMediator.m_CreateShapeManager.ClearStageData);
            m_SaveBtn.onClick.AddListener(CreatorMediator.m_CreateShapeManager.SaveStageDataClick);
            m_LoadBtn.onClick.AddListener(CreatorMediator.m_CreateShapeManager.LoadStageDataClick);
            m_DeleteBtn.onClick.AddListener(CreatorMediator.m_CreateShapeManager.DeleteStageDataClick);

            m_TestPlayBtn.onClick.AddListener(CreatorMediator.m_CreateShapeManager.TestStageDataClick);
            m_StopTestBtn.onClick.AddListener(CreatorMediator.m_CreateShapeManager.StopTestStageDataClick);
        }

        public void SetState(bool isEditing)
        {
            if (isEditing)
            {
                m_StateBG.color = Color.green;
                m_StateText.SetText("ステージ編集中");
            }
            else
            {
                m_StateBG.color = Color.red;
                m_StateText.SetText("テストプレイ中");
            }
        }

        public void SetTapCount(int cur, int max)
        {
            if (cur >= 0)
            {
                m_TapCountText.SetText($"{cur}/{max}");
            }
            else
            {
                m_TapCountText.SetText($"-/-");
            }
        }
    }
}
#endif