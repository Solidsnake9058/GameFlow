using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityGame.Data
{
    public abstract class SettingBaseEditor : Editor
    {
        public const string ControlTextName = "ControlTextName";

        protected SettingBase m_Asset;
        protected Editor m_Editor;
        protected Vector2 m_ScrollPosition;
        protected string m_Path;

        public int Order { get { return m_Asset.ShowOrder; } }

        public virtual void Setup(SettingBase asset)
        {
            m_Asset = asset;
            m_Editor = CreateEditor(asset) as Editor;
        }

        public abstract void OnGUI();
        public virtual void OnPreSave() { }
        public virtual void OnPostSave() { }
        public virtual void OnDestroy() { }

        public void SetAssetPath(string path)
        {
            m_Path = path;
        }

        protected void BeginGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            GUI.SetNextControlName(ControlTextName);
            EditorGUILayout.PropertyField(m_Editor.serializedObject.FindProperty("SettingName"), new GUIContent("Object Name"), true);

            GUILayout.Space(20);
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
        }

        protected void EndGUI()
        {
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                m_Editor.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_Asset);
            }
        }

        public string GetSettingName()
        {
            return m_Asset?.SettingName;
        }
    }
}