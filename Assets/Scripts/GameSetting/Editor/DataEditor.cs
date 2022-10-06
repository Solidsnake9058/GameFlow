using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnityGame.Data
{
    public class DataEditor : EditorWindow
    {
        private const string m_SettingPath = "Assets/GameSettingDatas";
        private List<SettingBaseEditor> m_DataEditor = new List<SettingBaseEditor>();
        private static string[] m_SelectText;
        private int m_SelectedIndex;

        private ListView m_ListView;
        private TextField m_SettingName;
        private VisualElement m_ContentView;

        private Button m_SaveButton;

        [MenuItem("GameSetting/DataEditor")]
        public static void ShowExample()
        {
            DataEditor wnd = GetWindow<DataEditor>();
            wnd.titleContent = new GUIContent("DataEditor");
        }

        public void CreateGUI()
        {
            Setup();
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/GameSetting/Editor/DataEditor.uxml");
            visualTree.CloneTree(root);

            m_ListView = root.Q<ListView>("SettingListView");
            m_ListView.makeItem = MakeListItem;
            m_ListView.bindItem = BindListItem;
            m_ListView.itemsSource = m_DataEditor;
            m_ListView.onSelectionChange += OnListSelectionChange;

            m_SettingName = root.Q<TextField>("SettingName");
            m_ContentView = root.Q<VisualElement>("ContentView");

            m_SaveButton = root.Q<Button>("SaveButton");
            m_SaveButton.clicked += SaveClick;
        }

        private void SaveClick()
        {
            m_DataEditor[m_SelectedIndex]?.OnPreSave();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            m_DataEditor[m_SelectedIndex]?.OnPostSave();
        }

        private void OnListSelectionChange(IEnumerable<object> obj)
        {
            m_SettingName.value = "";
            m_ContentView.Clear();
            foreach (var item in obj)
            {
                SettingBaseEditor editor = item as SettingBaseEditor;
                editor.CreateGUI();
                m_SelectedIndex = m_DataEditor.IndexOf(editor);
                m_SettingName.value = editor.GetSettingName();
                m_ContentView.Add(editor.m_Root);
            }
        }

        private void BindListItem(VisualElement item, int index)
        {
            Label label = item as Label;
            SettingBaseEditor editor = m_DataEditor[index];
            label.text = editor.GetSettingName();
        }

        private VisualElement MakeListItem()
        {
            Label label = new Label();
            return label;
        }

        private void Setup()
        {
            var guids = AssetDatabase.FindAssets("", new string[] { m_SettingPath });
            guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToList().ForEach(path =>
            {
                var data = AssetDatabase.LoadAssetAtPath<SettingBase>(path);
                if (data != null)
                {
                    Type type = Type.GetType($"UnityGame.Data.{data.GetEditorName()}");
                    if (!m_DataEditor.Where(x => x.GetType().Equals(type)).Any())
                    {
                        var editor = Activator.CreateInstance(type) as SettingBaseEditor;
                        editor.Setup(data);
                        //editor.SetAssetPath(path);
                        m_DataEditor.Add(editor);

                    }
                }
            });
            m_DataEditor = m_DataEditor.OrderByDescending(x => x.Order).ToList();
        }
    }
}