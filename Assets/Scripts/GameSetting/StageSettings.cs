using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.Data;
using System.Linq;
//using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif

namespace UnityGame.Data
{
    [CreateAssetMenu(fileName = "StageSettings", menuName = "SettingData/StageSettings")]
    public class StageSettings : SettingBase
    {
        public List<StageSetting> m_StageDatas;

        public StageSetting GetStageSetting(int level)
        {
            for (int i = 0; i < m_StageDatas.Count; i++)
            {
                if (m_StageDatas[i].m_StageNo == level)
                {
                    return m_StageDatas[i];
                }
            }
            return null;
        }

#if UNITY_EDITOR
        public override string GetEditorName()
        {
            return "StageSettingsEditor";
        }

        public void LoadPrefab()
        {
            m_StageDatas.ForEach(x =>
            {
                if (x.m_StageData == null)
                    x.m_StageData = AssetDatabase.LoadAssetAtPath<StageData>(AssetDatabase.GUIDToAssetPath(x.m_Guid));
            });
            //OrderBonus.ForEach(x =>
            //{
            //    if (x.Prefab == null)
            //        x.Prefab = AssetDatabase.LoadAssetAtPath<StageData>(AssetDatabase.GUIDToAssetPath(x.Guid));
            //});
        }

        public void CreateGroup()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
            var groupName = "Stages";
            var group = settings.FindGroup(groupName);
            if (group == null)
            {
                var schema = new List<AddressableAssetGroupSchema>() { new BundledAssetGroupSchema(), new ContentUpdateGroupSchema() };
                group = settings.CreateGroup(groupName, false, false, true, schema, schema.GetType());
            }
            group.entries.ToList().ForEach(x => group.RemoveAssetEntry(x));
            var entryAndRename = new Action<StageSetting>(x =>
            {
                var entry = settings.CreateOrMoveEntry(x.m_Guid, group);
                entry.address = x.m_Name;
            });
            m_StageDatas.ForEach(x => entryAndRename(x));
            //OrderBonus.ForEach(x => entryAndRename(x));

            AssetDatabase.SaveAssets();
        }
#endif

        [System.Serializable]
        public class StageSetting : SortingObject
        {
            public int m_StageNo;
            public string m_Guid;
            public string m_Name;
            public int m_FailToStage = 0;
            public bool m_IsCheck;
#if UNITY_EDITOR
            public StageData m_StageData { get; set; }
#endif
        }
    }

}