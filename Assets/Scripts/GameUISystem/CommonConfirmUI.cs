using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UnityGame.App.UI
{
    public class CommonConfirmUI : IGameUISystem
    {
        [SerializeField]
        private TextMeshProUGUI m_ActionText;
        [SerializeField]
        private Button m_ConfirmBtn;
        [SerializeField]
        private Button m_CancelBtn;

        private Action m_ConfirmAction;
        private Action m_CancelAction;

        private void Start()
        {
            m_ConfirmBtn.onClick.AddListener(ConformClick);
            m_CancelBtn.onClick.AddListener(CancelClick);
        }

        public void SetConfirmAction(string desc,Action action)
        {
            SetConfirmAction(desc,"はい","いいえ",action,null);
        }

        public void SetConfirmAction(string desc,string confirmText, string cancelText, Action confirmAction, Action cancelAction)
        {
            m_ActionText.SetText(desc);
            m_ConfirmBtn.GetComponentInChildren<TextMeshProUGUI>().SetText(confirmText);
            m_CancelBtn.GetComponentInChildren<TextMeshProUGUI>().SetText(cancelText);
            m_ConfirmAction = confirmAction;
            m_CancelAction = cancelAction;
            ShowUI();
        }

        private void ConformClick()
        {
            m_ConfirmAction?.Invoke();
            m_CancelAction = null;
            m_ConfirmAction = null;
            HideUI();
        }

        private void CancelClick()
        {
            m_CancelAction?.Invoke();
            m_CancelAction = null;
            m_ConfirmAction = null;
            HideUI();
        }
    }
}