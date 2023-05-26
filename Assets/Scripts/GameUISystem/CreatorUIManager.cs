#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.App.Manager
{
    using System;
    using UnityGame.App.Actor;
    using UnityGame.App.UI;

    public class CreatorUIManager : IGameItem
    {
        [SerializeField]
        private StageFileUI m_StageFileUI;
        [SerializeField]
        private CommonConfirmUI m_CommonConfirmUI;

        public void SetState(bool isEditing) => m_StageFileUI.SetState(isEditing);
        public void SetTapCount(int cur, int max) => m_StageFileUI.SetTapCount(cur, max);

        public void SetConfirmAction(string desc, Action action) => m_CommonConfirmUI.SetConfirmAction(desc, action);
        public void SetConfirmAction(string desc, string confirmText, string cancelText, Action confirmAction, Action cancelAction) => m_CommonConfirmUI.SetConfirmAction(desc, confirmText, cancelText, confirmAction, cancelAction);

    }
}
#endif