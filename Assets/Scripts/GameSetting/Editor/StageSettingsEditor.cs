using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityGame.App.Editor;

namespace UnityGame.Data
{
    using static UnityEditor.Progress;
    using System.IO;
    using System.Linq;

    public class StageSettingsEditor : SettingBaseEditor
    {
        private StageSettings stageSettings;
        private ListView m_StageListView;
        private Button m_AddStageButton;
        private StageData m_OpenAsset;

        private SystemLanguage m_DebugSystemLang;

        public override void CreateGUI()
        {
            if (m_Root == null)
            {
                m_Root = rootVisualElement;
                stageSettings = m_Asset as StageSettings;
                SerializedObject so = new SerializedObject(stageSettings);
                // Import UXML
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/GameSetting/Editor/StageSettingsEditor.uxml");
                visualTree.CloneTree(m_Root);

                var stageFoldout = m_Root.Q<Foldout>("stage-foldout");
                var labelText = stageFoldout.Q<Label>();
                labelText?.AddToClassList(CustomFoldOutClass);
                m_StageListView = m_Root.Q<ListView>("stageListView");
                m_AddStageButton = m_Root.Q<Button>("addStageButton");

                m_StageListView.showAddRemoveFooter = true;
                m_StageListView.makeItem = MakeStageItem;
                m_StageListView.bindItem = BindStageListItem;
                m_StageListView.itemsSource = stageSettings.m_StageDatas;
                m_AddStageButton.clicked += AddStageData;
            }
        }

        public void ReSetData()
        {
            for (int i = 0; i < stageSettings.m_StageDatas.Count; i++)
            {
                if (stageSettings.m_StageDatas[i].m_StageData == null)
                {
                    stageSettings.m_StageDatas.Remove(stageSettings.m_StageDatas[i]);
                }
            }
        }

        public void RefreshItems()
        {
            m_StageListView.RefreshItems();
        }

        private void BindStageListItem(VisualElement item, int index)
        {
            StageItemEditor stageItemEditor = item as StageItemEditor;
            stageItemEditor.SetStageItem(this, index, stageSettings.m_StageDatas[index]);
        }

        private VisualElement MakeStageItem()
        {
            StageItemEditor stageItemEditor = new StageItemEditor();
            return stageItemEditor;
        }

        public bool CheckOpenAsset(StageData stageData)
        {
            return m_OpenAsset != null && m_OpenAsset.Equals(stageData);
        }

        public void RemoveStage(int stageNo)
        {
            for (int i = 0; i < stageSettings.m_StageDatas.Count; i++)
            {
                if (stageSettings.m_StageDatas[i].m_StageNo.Equals(stageNo))
                {
                    stageSettings.m_StageDatas.Remove(stageSettings.m_StageDatas[i]);
                    break;
                }
            }
            m_StageListView.Rebuild();
        }

        public void SetOpenAsset(StageData stageData)
        {
            m_OpenAsset = stageData;
            m_StageListView.RefreshItems();
        }

        private void AddStageData()
        {
            var path = "Assets/StageData/Stage";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var files = Directory.GetFiles($"{path}/", "*.prefab");
            var data = m_Asset as StageSettings;
            files.ToList().ForEach(x =>
            {
                var guid = AssetDatabase.AssetPathToGUID(x);
                if (!data.m_StageDatas.Any(y => y.m_Guid.Equals(guid)))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<StageData>(AssetDatabase.GUIDToAssetPath(guid));
                    data.m_StageDatas.Add(new StageSettings.StageSetting()
                    {
                        m_StageNo = data.m_StageDatas.Count == 0 ? 1 : data.m_StageDatas.Max(y => y.m_StageNo) + 1,
                        m_Guid = guid,
                        m_Name = prefab.name,
                        m_StageData = prefab
                    });
                }
            });

            m_StageListView.Rebuild();
        }

        public override void OnPostSave()
        {
            base.OnPostSave();
            (m_Asset as StageSettings).CreateGroup();
        }
    }
}
