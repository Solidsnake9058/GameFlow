using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;
using UnityEditor.VersionControl;
using UnityEditor.SceneManagement;

namespace UnityGame.Data
{
    using static UnityGame.Data.StageSettings;

    public class StageItemEditor : VisualElement
    {
        private const string UXMLPATH = "Assets/Scripts/GameSetting/Editor/StageItemEditor.uxml";

        private StageSettingsEditor m_StageSettingsEditor;
        private StageSetting m_StageSetting;
        private Button m_RemoveButton;
        private Label m_ShowNO;
        private ObjectField m_PrefabField;
        private Label m_InnerNO;
        private Button m_OpenStageButton;
        private Button m_PlayStageButton;

        public StageItemEditor()
        {
            Init();
        }

        public void Init()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXMLPATH);
            visualTree.CloneTree(this);

            m_RemoveButton = this.Q<Button>("removeButton");
            m_ShowNO = this.Q<Label>("showNO");
            m_PrefabField = this.Q<ObjectField>("prefabField");
            m_PrefabField.objectType = typeof(StageData);
            m_PrefabField.bindingPath = "m_StageData";
            m_InnerNO = this.Q<Label>("innerNO");
            m_PrefabField.bindingPath = "m_StageNo";
            m_OpenStageButton = this.Q<Button>("openStageButton");
            m_PlayStageButton = this.Q<Button>("playStageButton");

            m_RemoveButton.clicked += RemoveStageClick;
            m_OpenStageButton.clicked += OpenStageClick;
            m_PlayStageButton.clicked += PlayStageClick;
        }

        public void SetStageItem(StageSettingsEditor stageSettingsEditor, int showNO, StageSetting stageSetting)
        {
            m_StageSettingsEditor = stageSettingsEditor;
            m_StageSetting = stageSetting;

            m_ShowNO.text = $"{showNO}";
            if (stageSetting.m_StageData == null && !String.IsNullOrWhiteSpace(stageSetting.m_Guid))
            {
                stageSetting.m_StageData = AssetDatabase.LoadAssetAtPath<StageData>(AssetDatabase.GUIDToAssetPath(stageSetting.m_Guid));
            }
            m_PrefabField.value = stageSetting.m_StageData;
            m_PrefabField.RegisterValueChangedCallback(CheckPrafab);
            m_InnerNO.text = $"{stageSetting.m_StageNo}";
            SetButton();
        }

        private void CheckPrafab(ChangeEvent<UnityEngine.Object> evt)
        {
            if (evt.newValue != null)
            {
                string newGuid;
                long newLocalId;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(evt.newValue,out newGuid, out newLocalId))
                {
                    m_StageSetting.m_Guid = newGuid;
                    m_StageSetting.m_Name = evt.newValue.name;
                }
            }
            m_StageSettingsEditor.RefreshItems();
        }

        private void SetButton()
        {
            //if (m_StageSettingsEditor.CheckOpenAsset(m_StageSetting.m_StageData))
            //{
            //    m_OpenStageButton.AddToClassList("open-asset");
            //}
            //else
            //{
            //    m_OpenStageButton.RemoveFromClassList("open-asset");
            //}
            m_OpenStageButton.EnableInClassList("open-asset", m_StageSettingsEditor.CheckOpenAsset(m_StageSetting.m_StageData));
            m_OpenStageButton.SetEnabled(!EditorApplication.isPlaying);
        }

        private void RemoveStageClick()
        {
            m_StageSettingsEditor.RemoveStage(m_StageSetting.m_StageNo);
        }

        private void OpenStageClick()
        {
            if (!EditorApplication.isPlaying)
            {
                AssetDatabase.OpenAsset(m_StageSetting.m_StageData);
                m_StageSettingsEditor.SetOpenAsset(m_StageSetting.m_StageData);
            }
        }

        private void PlayStageClick()
        {
            //EditorPrefs.SetInt("GameMode", (int)mode);
            EditorPrefs.SetInt("DebugStageNo", m_StageSetting.m_StageNo);

            if (!EditorApplication.isPlaying)
            {
                if (EditorSceneManager.GetActiveScene().name.IndexOf("Game") < 0)
                {
                    var path = $"Assets/Scenes/Game.unity";
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(path);
                }
                EditorApplication.isPlaying = true;
            }
        }

        public class Factory : UxmlFactory<StageItemEditor, Traits> { }

        public class Traits : UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
    }
}