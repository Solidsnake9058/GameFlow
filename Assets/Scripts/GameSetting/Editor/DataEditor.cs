using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UnityGame.Data
{
    public class DataEditor : EditorWindow
    {
        private static List<SettingBaseEditor> m_DataEditor;
        private static GUISkin m_Skin;
        private static string[] m_SelectText;

        private int _SelectedIndex;
        private Vector2 _ScrollPos;

        [MenuItem("GameSetting/DataEditor")]
        private static void Open()
        {
            var window = GetWindow<DataEditor>("DataEditor");
            window.minSize = new Vector2(900, 300);
            m_DataEditor = null;
        }

        public void OnGUI()
        {
            Setup();

            EditorGUILayout.BeginHorizontal();

            var skin = GUI.skin;
            GUI.skin = m_Skin;
            EditorGUILayout.BeginVertical(GUILayout.Width(130));
            var texts = m_SelectText;
            if (texts == null || GUI.GetNameOfFocusedControl() != SettingBaseEditor.ControlTextName)
            {
                texts = m_DataEditor.Select(editor => editor.GetSettingName()).ToArray();
                m_SelectText = texts;
            }

            _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);
            _SelectedIndex = GUILayout.SelectionGrid(_SelectedIndex, texts, 1);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            GUI.skin = skin;

            if (m_DataEditor.Count > 0)
            {
                m_DataEditor[_SelectedIndex]?.OnGUI();
            }

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("セーブ"))
            {
                m_DataEditor[_SelectedIndex]?.OnPreSave();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                m_DataEditor[_SelectedIndex]?.OnPostSave();
            }
        }

        private void Setup()
        {
            if (!m_Skin)
            {
                m_Skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/Scripts/GameSetting/Editor/DataEditorListSkin.guiskin", typeof(GUISkin));
            }
            if (m_DataEditor == null || m_DataEditor.Count == 0)
            {
                m_DataEditor = new List<SettingBaseEditor>();
                var guids = AssetDatabase.FindAssets("", new string[] { "Assets/GameSettingDatas" });

                guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToList().ForEach(path =>
                {
                    var data = AssetDatabase.LoadAssetAtPath<SettingBase>(path);
                    if (data != null)
                    {
                        Type type = Type.GetType($"UnityGame.Data.{data.GetEditorName()}");
                        var editor = Activator.CreateInstance(type) as SettingBaseEditor;
                        editor.Setup(data);
                        //editor.SetAssetPath(path);
                        m_DataEditor.Add(editor);
                    }
                });
                m_DataEditor = m_DataEditor.OrderByDescending(x => x.Order).ToList();
            }
        }

        private void OnDestroy()
        {
            m_DataEditor.ForEach(x => x.OnDestroy());
        }
    }
}