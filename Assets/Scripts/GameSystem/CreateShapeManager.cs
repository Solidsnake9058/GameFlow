#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGame.App.Manager
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityGame.App.Actor;
    using Newtonsoft.Json;

    public class CreateShapeManager : IGameItem
    {
        [SerializeField]
        private InputAction m_MouseClick;
        [SerializeField]
        private Transform m_Background;
        [SerializeField]
        private Transform m_ShapeContent;
        [SerializeField]
        private Vector2 m_ScreenSize = new Vector2(1080, 1920);
        [SerializeField]
        private Vector2 m_CopyOffset;
        public Vector3 m_CopyOffset3 => m_CopyOffset;
        private Camera m_MainCamera;

        private Vector3 m_DragRange;
        private Vector3 m_LimitMax;
        private int _TapCount = 1;
        public int m_TapCount => _TapCount;

        private const string m_StageFileFormat = "Stage_{0}.txt";
        private const string m_StagePath = "Assets/StageData/Stage";
        private List<TextAsset> m_StageTextFile = new List<TextAsset>();
        private Dictionary<int, StagePatten> m_StagePattens = new Dictionary<int, StagePatten>();
        private int m_MaxStageIndexCur = 0;
        private int m_StageEditCur = 0;
        private StageData _TestPlayStageData;
        public StageData m_TestPlayStageData => _TestPlayStageData;

        [SerializeField]
        private StageEditButton m_StageEditButtonPrefab;
        [SerializeField]
        private Transform m_StageEditBtnContent;
        private List<StageEditButton> m_StageEditButtons = new List<StageEditButton>();
        private StageEditButton m_SelectedStage = null;
        private StageEditButton m_EditngStage = null;
        private CreatorMode _CreatorMode = CreatorMode.Editing;
        public CreatorMode m_CreatorMode => _CreatorMode;
        public bool m_IsEditing => m_CreatorMode.Equals(CreatorMode.Editing);

        public int MaxStageIndex => m_StagePattens.Select(x => x.Key).DefaultIfEmpty(0).Max();

        private void Awake()
        {
            m_MainCamera = Camera.main;
            m_DragRange = m_Background.localScale;
        }

        private void Start()
        {
            GetStageList();
        }

        private void OnEnable()
        {
            m_MouseClick.Enable();
            m_MouseClick.performed += MousePressed;
        }

        private void OnDisable()
        {
            m_MouseClick.performed -= MousePressed;
            m_MouseClick.Disable();
        }

        private void MousePressed(InputAction.CallbackContext context)
        {
            if (m_CreatorMode.Equals(CreatorMode.Editing))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);
                {
                    if (hit.collider != null)
                    {
                        StartCoroutine(DragUpdate(hit.collider.gameObject));
                    }
                }
            }
        }

        private IEnumerator DragUpdate(GameObject clickObject)
        {
            Vector3 initDis = clickObject.transform.position - Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                yield return null;
        }

        public static Vector3 ClampPosition(Vector3 value, Vector3 min, Vector3 max)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.x, max.y);
            value.z = Mathf.Clamp(value.z, min.x, max.z);
            return value;
        }


        public void SetTapCount(int value)
        {
            _TapCount = value;
        }

        private void CrearStageList()
        {
            for (int i = 0; i < m_StageEditButtons.Count; i++)
            {
                Destroy(m_StageEditButtons[i].gameObject);
            }
            m_StageEditButtons.Clear();
        }

        public void GetStageList()
        {
            CrearStageList();
            m_StagePattens.Clear();

            if (!Directory.Exists(m_StagePath))
            {
                Directory.CreateDirectory(m_StagePath);
            }
            var files = Directory.GetFiles($"{m_StagePath}/", "*.txt");
            string patterm = $"^{m_StagePath}/{string.Format(m_StageFileFormat, "[0-9]{4}")}$";

            files.ToList().ForEach(x =>
            {
                if (Regex.IsMatch(x, patterm))
                {
                    string indexStr = x.Replace($"{m_StagePath}/Stage_", "").Replace(".txt", "");
                    int index = 0;
                    if (int.TryParse(indexStr, out index))
                    {
                        var guid = AssetDatabase.AssetPathToGUID(x);
                        var json = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guid));
                        StageData stage = null;
                        //var stage =
                        try
                        {
                            stage = JsonConvert.DeserializeObject<StageData>(json.text);
                        }
                        catch (Exception ex)
                        {

                        }
                        var stagePtn = new StagePatten()
                        {
                            m_Index = index,
                            m_Guid = guid,
                            m_StageData = stage
                        };
                        m_StagePattens.Add(index, stagePtn);
                        var btn = Instantiate(m_StageEditButtonPrefab, m_StageEditBtnContent, false);
                        btn.SetStageButton(stagePtn);
                        m_StageEditButtons.Add(btn);
                    }
                }
            });
        }

        public void StageShiftSort(bool isUpper)
        {
            if (m_SelectedStage == null)
            {
                return;
            }
            int index = m_SelectedStage.m_Index;
            int changeIndex = index + (isUpper ? -1 : 1);
            if ((isUpper && index > 0) || (!isUpper && index < MaxStageIndex))
            {
                string targetFile = string.Format(m_StageFileFormat, index.ToString("0000"));
                string changeFile = string.Format(m_StageFileFormat, changeIndex.ToString("0000"));
                string changeTempFile = $"{string.Format(m_StageFileFormat, index.ToString("0000"))}.temp";

                string targetPath = $"{m_StagePath}/{targetFile}";
                string changePath = $"{m_StagePath}/{changeFile}";
                string changeTempPath = $"{m_StagePath}/{changeTempFile}";

                var target = AssetDatabase.AssetPathToGUID(targetPath, AssetPathToGUIDOptions.OnlyExistingAssets);
                var change = AssetDatabase.AssetPathToGUID(changePath, AssetPathToGUIDOptions.OnlyExistingAssets);
                string trgGuid = AssetDatabase.GUIDToAssetPath(target);
                if (target != null)
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GUIDToAssetPath(trgGuid), changeTempFile);
                    if (change != null)
                    {
                        AssetDatabase.RenameAsset(changePath, targetFile);
                    }
                    AssetDatabase.RenameAsset(AssetDatabase.GUIDToAssetPath(trgGuid), changeFile);
                    var trgPat = m_StageEditButtons.Where(x => x.m_Index.Equals(index)).FirstOrDefault();
                    var chgPat = m_StageEditButtons.Where(x => x.m_Index.Equals(changeIndex)).FirstOrDefault();
                    trgPat?.SetIndex(changeIndex);
                    chgPat?.SetIndex(index);

                    if (chgPat != null)
                    {
                        int transIndex = m_SelectedStage.transform.GetSiblingIndex();
                        transIndex += isUpper ? -1 : 1;
                        m_SelectedStage.transform.SetSiblingIndex(transIndex);
                    }

                }
            }
        }

        public void StageButtonSelect(StageEditButton stageEditButton)
        {
            if (stageEditButton.Equals(m_SelectedStage))
            {
                stageEditButton.SetSelected(false);
                m_SelectedStage = null;
            }
            else
            {
                m_SelectedStage?.SetSelected(false);
                m_SelectedStage = stageEditButton;
                stageEditButton.SetSelected(true);
            }
        }

        public void ClearStageData()
        {
            if (!m_IsEditing)
            {
                return;
            }
        }

        public void SaveStageDataClick()
        {
            if (!m_IsEditing)
            {
                return;
            }
            if (m_SelectedStage != null)
            {
                int index = m_SelectedStage.m_Index;
                CreatorMediator.m_CreatorUIManager.SetConfirmAction($"\"Stage {index:0000}\"に上書きしますか?", SaveStageDataRewrite);
            }
            else
            {
                SaveStageDataNew();
            }
        }

        public void SaveStageDataNew()
        {
            int newIndex = m_StagePattens.Count;
            if (!MaxStageIndex.Equals(newIndex - 1))
            {
                newIndex = GetNewStageIndex();
            }
            StageData data = GetEditStageData();
            string path = $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"))}/{m_StagePath}/{string.Format(m_StageFileFormat, newIndex.ToString("0000"))}";
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(JsonConvert.SerializeObject(data));
                sw.Close();
            }
            AssetDatabase.Refresh();
            GetStageList();
        }

        private void SaveStageDataRewrite()
        {
            int index = m_SelectedStage.m_Index;
            StageData data = GetEditStageData();
            string path = $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"))}/{m_StagePath}/{string.Format(m_StageFileFormat, index.ToString("0000"))}";
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(JsonConvert.SerializeObject(data));
                sw.Close();
            }
            AssetDatabase.Refresh();
            if (m_StagePattens.ContainsKey(index))
            {
                m_StagePattens[index].m_StageData = data;
            }
        }

        private StageData GetEditStageData()
        {
            var data = new StageData
            {
            };
            return data;
        }

        public void LoadStageDataClick()
        {
            if (!m_IsEditing)
            {
                return;
            }
            if (m_SelectedStage != null)
            {
                ClearStageData();
                int index = m_SelectedStage.m_Index;
                StageData stageData = m_StagePattens[index].m_StageData;
                if (stageData != null)
                {
                }
            }
        }

        public void DeleteStageDataClick()
        {
            if (!m_IsEditing)
            {
                return;
            }
            if (m_SelectedStage != null)
            {
                int index = m_SelectedStage.m_Index;
                CreatorMediator.m_CreatorUIManager.SetConfirmAction($"\"Stage {index:0000}\"を消しますか?", DeleteStageData);
            }
        }

        private void DeleteStageData()
        {
            int index = m_SelectedStage.m_Index;
            string path = $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"))}/{m_StagePath}/{string.Format(m_StageFileFormat, index.ToString("0000"))}";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (m_StagePattens.ContainsKey(index))
            {
                m_StagePattens.Remove(index);
            }
            m_SelectedStage = null;
            AssetDatabase.Refresh();
            GetStageList();
        }

        public void TestStageDataClick()
        {
            _TestPlayStageData = GetEditStageData();
            if (m_IsEditing && CreatorMediator.m_GameManager.StartGame())
            {
                ChangeMode(CreatorMode.TestPlaying);
                m_ShapeContent.localPosition = new Vector3(0, 0, 2);
            }
        }

        public void ReTestStage()
        {
            //StopTest();
            ((CreatorStageManager)CreatorMediator.m_StageManager).ClearStage();
            CreatorMediator.m_GameManager.ToStart();
            if (CreatorMediator.m_GameManager.StartGame())
            {
                //ChangeMode(CreatorMode.TestPlaying);
                //m_ShapeContent.localPosition = new Vector3(0, 0, 2);
            }
        }

        public void StopTestStageDataClick()
        {
            StopTest();
        }

        public void StopTest()
        {
            if (m_CreatorMode.Equals(CreatorMode.TestPlaying))
            {
                CreatorMediator.m_GameManager.ToStart();
                _TestPlayStageData = null;
                m_ShapeContent.localPosition = Vector3.zero;
                ((CreatorStageManager)CreatorMediator.m_StageManager).ClearStage();
                CreatorMediator.m_CreatorUIManager.SetTapCount(-1, -1);
                ChangeMode(CreatorMode.Editing);
            }
        }

        private void ChangeMode(CreatorMode creatorMode)
        {
            _CreatorMode = creatorMode;
            CreatorMediator.m_CreatorUIManager.SetState(m_CreatorMode.Equals(CreatorMode.Editing));
        }

        private int GetNewStageIndex()
        {
            for (int i = 0; i <= MaxStageIndex; i++)
            {
                if (m_StagePattens.Where(x => x.Key.Equals(i)).Count() == 0)
                {
                    return i;
                }
            }
            return m_StagePattens.Count;
        }
    }

    [Serializable]
    public class StagePatten
    {
        public int m_Index;
        public string m_Guid;
        public StageData m_StageData;

        public void SetIndex(int index)
        {
            m_Index = index;
        }
    }
}
#endif