using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityGame.Data
{
    public abstract class SettingBaseEditor : EditorWindow
    {
        public const string ControlTextName = "ControlTextName";
        protected const string CustomFoldOutClass = "group-foldout-field";

        protected SettingBase m_Asset;
        protected EditorWindow m_Editor;
        public VisualElement m_Root;
        protected Vector2 m_ScrollPosition;
        protected string m_Path;

        public int Order { get { return m_Asset.ShowOrder; } }

        public virtual void Setup(SettingBase asset)
        {
            m_Asset = asset;
            //m_Editor = CreateEditor(asset) as EditorWindow;
            CreateGUI();
        }

        public abstract void CreateGUI();
        public virtual void OnPreSave() { }
        public virtual void OnPostSave() { }

        public string GetSettingName()
        {
            return m_Asset?.SettingName;
        }
    }
}