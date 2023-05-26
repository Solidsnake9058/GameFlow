using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityGame.Data;
using System.Linq;

namespace UnityGame.Data
{
    public class AppSettingEditor : SettingBaseEditor
    {
        ListView m_TenjinEventStage;

        public override void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            if (m_Root == null)
            {
                m_Root = rootVisualElement;
                AppSetting appSetting = m_Asset as AppSetting;
                SerializedObject so = new SerializedObject(appSetting);
                // Import UXML
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/GameSetting/Editor/SettingBaseEditor.uxml");
                visualTree.CloneTree(m_Root);

                ScrollView scrollView = m_Root.Q<ScrollView>();

                #region Ad
                Foldout adField = new FoldoutExtend("Ad data", CustomFoldOutClass);
                adField.Add(new SliderIntExtend("Interstitial Start Stage", "AD.AdInterstitialStartStageNo", so, true, 1, 100));
                adField.Add(new SliderIntExtend("Interstitial Interval", "AD.AdInterstitialInterval", so, true, 0, 100));
                #endregion

                #region Analytics
                Foldout AnalyticsDataField = new FoldoutExtend("Analytics", CustomFoldOutClass);
                AnalyticsDataField.Add(new PropertyFieldExtend("Tenjin Event Stage", "Analytics.TenjinClearEventStages", so));
                #endregion

                scrollView.Add(adField);
                scrollView.Add(AnalyticsDataField);
            }

        }
    }
}