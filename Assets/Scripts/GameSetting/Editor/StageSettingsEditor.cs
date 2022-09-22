using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace UnityGame.Data
{
    using App.Editor;
    using UnityEditor.SceneManagement;

    public class StageSettingsEditor : SettingBaseEditor
    {
        private const int m_CellWidth = 100;
        private const int m_ObjectCellWidth = 150;

        private Vector2 m_ScrollPosStageList;
        private ReorderableListEx m_Reorderable = new ReorderableListEx();
        private EditorWindow m_Window;
        private StageData m_OpenAsset;

        public override void Setup(SettingBase asset)
        {
            base.Setup(asset);

            var data = m_Asset as StageSettings;

            m_Reorderable.Setup(m_Editor.serializedObject, m_Editor.serializedObject.FindProperty("m_StageDatas"), data.m_StageDatas);
            m_Reorderable.drawElementCallback((ref Rect rect, int i) =>
            {
                if (i < data.m_StageDatas.Count)
                    Show(i, data.m_StageDatas[i], ref rect, App.GameMode.Default);
            });
            m_Reorderable.ReorderableList.onAddCallback = x =>
            {
                data.m_StageDatas.Add(new StageSettings.StageSetting() { m_StageNo = data.m_StageDatas.Count() == 0 ? 1 : data.m_StageDatas.Max(y => y.m_StageNo) + 1 });
            };
            m_Reorderable.ReorderableList.onReorderCallback = _ =>
            {
                data.m_StageDatas.ForEach(x =>
                {
                    if (!x.m_Name.Equals(x.m_StageData.name))
                        x.m_StageData = AssetDatabase.LoadAssetAtPath<StageData>(AssetDatabase.GUIDToAssetPath(x.m_Guid));
                });
            };

            m_Window = EditorWindow.GetWindow<DataEditor>("DataEditor");
            EditorApplication.playModeStateChanged += mode =>
            {
                switch (mode)
                {
                    case PlayModeStateChange.EnteredPlayMode:
                    case PlayModeStateChange.EnteredEditMode:
                        if (EditorPrefs.HasKey("DataStageScroll"))
                            m_ScrollPosition.y = EditorPrefs.GetFloat("DataStageScroll");
                        m_OpenAsset = null;
                        break;

                    case PlayModeStateChange.ExitingPlayMode:
                    case PlayModeStateChange.ExitingEditMode:
                        EditorPrefs.SetFloat("DataStageScroll", m_ScrollPosition.y);
                        break;
                }
            };
            if (EditorPrefs.HasKey("DataStageScroll"))
                m_ScrollPosition.y = EditorPrefs.GetFloat("DataStageScroll");

            data.LoadPrefab();
        }

        public override void OnGUI()
        {
            var data = m_Asset as StageSettings;

            BeginGUI();

            //var propStages = m_Editor.serializedObject.FindProperty("m_StageDatas");
            GUILayout.Space(10);

            m_Reorderable.Update();
            m_Reorderable.SetScrollRect(0, m_ScrollPosition.y, m_Window.position.width, m_Window.position.height);
            //m_ReorderableBonus.Update();
            //m_ReorderableBonus.SetScrollRect(0, _ScrollPos.y, _Window.position.width, _Window.position.height);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("表示番号", GUILayout.Width(50));
                EditorGUILayout.LabelField("ステージ名", GUILayout.Width(90));
                EditorGUILayout.LabelField("内部番号", GUILayout.Width(130));
                EditorGUILayout.LabelField("", GUILayout.Width(50));
                EditorGUILayout.LabelField("失敗へステージ", GUILayout.Width(90));
            }
            EditorGUILayout.EndHorizontal();
            m_Reorderable.Show();

            //EditorGUILayout.Space();
            //EditorGUILayout.Space();
            //EditorGUILayout.BeginHorizontal();
            //{
            //    if (GUILayout.Button("Add", GUILayout.Width(200)))
            //    {
            //        propStages.arraySize++;
            //        var prop = propStages.GetArrayElementAtIndex(propStages.arraySize - 1);
            //        var propIndex = prop.FindPropertyRelative("m_StageIndex");
            //        propIndex.intValue = maxIndex + 1;
            //    }
            //    if (propStages.arraySize > 0 && GUILayout.Button("Delete", GUILayout.Width(200)))
            //    {
            //        propStages.arraySize--;
            //    }
            //}
            //EditorGUILayout.EndHorizontal();

            //Debug.Log(Event.current.type);


            ShowSubButton();
            EndGUI();
        }

        private void Show(int index, StageSettings.StageSetting asset, ref Rect rect, App.GameMode mode)
        {
            // No
            var width = 40;
            rect.size = new Vector2(width, 17);
            EditorGUI.LabelField(rect, $"{index + 1}");
            rect.x += width;

            // Prefab
            width = 100;
            rect.size = new Vector2(width, 17);
            if (asset.m_StageData == null)
                asset.m_StageData = AssetDatabase.LoadAssetAtPath<StageData>(AssetDatabase.GUIDToAssetPath(asset.m_Guid));
            var prefab = EditorGUI.ObjectField(rect, asset.m_StageData, typeof(StageData), false) as StageData;
            if (prefab != asset.m_StageData || (asset.m_Name != null && !asset.m_Name.Equals(prefab?.name)))
            {
                asset.m_StageData = prefab;
                asset.m_Guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset.m_StageData));
                asset.m_Name = asset.m_StageData?.name;
                EditorUtility.SetDirty(m_Asset);
            }
            rect.x += width;

            GUI.enabled = false;
            // Stage No
            width = 50;
            rect.size = new Vector2(width, 17);
            asset.m_StageNo = EditorGUI.IntField(rect, asset.m_StageNo);
            rect.x += width;
            GUI.enabled = true;

            GUI.enabled = !EditorApplication.isPlaying;
            // open
            width = 50;
            rect.x += 30;
            rect.size = new Vector2(width, 17);
            var defaultColor = GUI.color;
            if (m_OpenAsset != null && asset.m_StageData == m_OpenAsset)
                GUI.color = Color.blue;
            if (GUI.Button(rect, "open"))
            {
                AssetDatabase.OpenAsset(asset.m_StageData);
                m_OpenAsset = asset.m_StageData;
            }
            GUI.color = defaultColor;
            rect.x += width;
            GUI.enabled = true;

            // play
            width = 50;
            rect.size = new Vector2(width, 17);
            if (GUI.Button(rect, "play"))
            {
                EditorPrefs.SetInt("GameMode", (int)mode);
                EditorPrefs.SetInt("DebugStageNo", asset.m_StageNo);

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
                else
                {
                    //scene.Load("Game");
                }
            }
            rect.x += width;

            //Fail Stage No
            width = 90;
            rect.size = new Vector2(width, 17);
            asset.m_FailToStage = EditorGUI.IntField(rect, asset.m_FailToStage);
            rect.x += width;
            GUI.enabled = true;

            // check
            width = 17;
            rect.x += 20;
            rect.size = new Vector2(width, 17);
            asset.m_IsCheck = EditorGUI.Toggle(rect, asset.m_IsCheck);
            rect.x += width;

#if true
            // playing
            //if (EditorApplication.isPlaying &&
            //    game != null && asset.m_StageIndex == game.StageNo &&
            //    GameMode == mode)
            //{
            //    var color = GUI.color;
            //    GUI.color = Color.red;
            //    width = 100;
            //    rect.x += 20;
            //    rect.size = new Vector2(width, 17);
            //    EditorGUI.LabelField(rect, "Playing!!");
            //    rect.x += width;
            //    GUI.color = color;
            //}
#endif
        }

        private void ShowSubButton()
        {
            GUILayout.Space(50);
            if (GUILayout.Button("データ追加"))
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
                            m_StageData = prefab,
                            m_FailToStage = 0
                        });
                    }
                });

                //path = "Assets/StageData/Bonus";
                //files = Directory.GetFiles($"{path}/", "*.prefab");
                //files.ToList().ForEach(x =>
                //{
                //    var guid = AssetDatabase.AssetPathToGUID(x);
                //    if (!data.OrderBonus.Any(y => y.Guid.Equals(guid)))
                //    {
                //        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                //        data.OrderBonus.Add(new StageSettings.OrderData()
                //        {
                //            StageNo = data.OrderBonus.Count == 0 ? 1 : data.OrderBonus.Max(y => y.StageNo) + 1,
                //            Guid = guid,
                //            Name = prefab.name,
                //            Prefab = prefab,
                //        });
                //    }
                //});
            }
        }

        public override void OnPostSave()
        {
            base.OnPostSave();
            (m_Asset as StageSettings).CreateGroup();
        }
    }
}