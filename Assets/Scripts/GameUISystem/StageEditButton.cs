#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityGame.App.Manager;
using UnityEngine.UI;

namespace UnityGame.App.Actor
{
    public class StageEditButton : IGameItem
    {
        [SerializeField]
        private GameObject m_EditMark;
        [SerializeField]
        private TextMeshProUGUI m_StageName;
        [SerializeField]
        private GameObject m_ReadFailedMark;
        [SerializeField]
        private Button m_Button;
        private StagePatten m_StagePatten;
        private bool m_IsSelected = false;
        private bool m_IsCanLoad = false;
        public int m_Index => m_StagePatten.m_Index;

        public void SetStageButton(StagePatten stagePatten)
        {
            m_StagePatten = stagePatten;
            m_EditMark.SetActive(false);
            m_StageName.SetText($"Stage {m_StagePatten.m_Index:0000}");
            m_IsCanLoad = m_StagePatten.m_StageData != null;
            m_ReadFailedMark.SetActive(!m_IsCanLoad);
            m_Button.onClick.AddListener(OnClickEvent);
        }

        private void OnClickEvent()
        {
            CreatorMediator.m_CreateShapeManager.StageButtonSelect(this);
        }

        public void SetSelected(bool isOn)
        {
            m_Button.image.color = isOn ? Color.gray : Color.white;
            m_IsSelected = isOn;
        }

        public void SetIndex(int index)
        {
            m_StagePatten?.SetIndex(index);
            m_StageName.SetText($"Stage {m_StagePatten.m_Index:0000}");
        }
    }
}
#endif