using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using static UnityGame.Data.StageSettings;
using UnityGame.App;

namespace UnityGame.Data
{
    public class TaskSettingEditor : SettingBaseEditor
    {
        private TaskSetting taskSetting;
        private ListView m_TaskListView;
        private Button m_RefreshDataBtn;
        private Dictionary<int, string> m_FoodList;
        private List<string> m_FoodNameList = new List<string>();
        private Dictionary<string, TaskEventType> m_DicTaskType;
        private List<string> m_TaskTypeList = new List<string>();
        private Dictionary<string, TaskShowType> m_DicTaskShowType;
        private List<string> m_TaskShowTypeList = new List<string>();

        public override void CreateGUI()
        {
            if (m_Root == null)
            {
                m_Root = rootVisualElement;
                taskSetting = m_Asset as TaskSetting;
                SerializedObject so = new SerializedObject(taskSetting);
                // Import UXML
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/GameSetting/Editor/TaskSettingEditor.uxml");
                visualTree.CloneTree(m_Root);

                m_DicTaskType = taskSetting.GetTaskTypeList();
                m_TaskTypeList = m_DicTaskType.Keys.ToList();
                m_DicTaskShowType = taskSetting.GetTaskShowTypeList();
                m_TaskShowTypeList = m_DicTaskShowType.Keys.ToList();

                m_RefreshDataBtn = m_Root.Q<Button>("refresh-data-btn");
                var taskFoldout = m_Root.Q<Foldout>("task-foldout");
                var labelText1 = taskFoldout.Q<Label>();
                labelText1?.AddToClassList(CustomFoldOutClass);
                m_TaskListView = m_Root.Q<ListView>("task-listview");

                m_RefreshDataBtn.clicked += RefreshData;
                //m_AddFoodButton.clicked += AddFoodData;
            }
        }

        private void RefreshData()
        {
            m_TaskListView.showAddRemoveFooter = true;
            m_TaskListView.makeItem = MakeTaskItem;
            m_TaskListView.bindItem = BindTaskListItem;
            m_TaskListView.itemsSource = taskSetting.Tasks;
            m_TaskListView.Rebuild();

            m_TaskListView.itemsAdded += (items) =>
            {
                var index = items.First();
                taskSetting.Tasks.RemoveAt(index);
                taskSetting.Tasks.Add(new TaskDetail(taskSetting.GetEmptyTaskId()));
                EditorUtility.SetDirty(taskSetting);
            };
        }

        public string GetFoodNameById(int id)
        {
            if (m_FoodList.ContainsKey(id))
            {
                return m_FoodList[id];
            }
            return "";
        }

        public int GetFoodIdByName(string name)
        {
            if (m_FoodList.ContainsValue(name))
            {
                return m_FoodList.FirstOrDefault(x => x.Value.Equals(name)).Key;
            }
            return -1;
        }

        public TaskEventType GetTaskEventType(string name)
        {
            if (m_DicTaskType.ContainsKey(name))
            {
                return m_DicTaskType[name];
            }
            return TaskEventType.TouchClick;
        }

        public string GetTaskEventTypeText(TaskEventType value)
        {
            var data = taskSetting.EventTexts.Where(x => x.EventType.Equals(value)).FirstOrDefault();
            if (data != null)
            {
                return data.TypeText;
            }
            return "";
        }

        public TaskShowType GetTaskShowType(string name)
        {
            if (m_DicTaskShowType.ContainsKey(name))
            {
                return m_DicTaskShowType[name];
            }
            return TaskShowType.WhenStart;
        }

        public string GetTaskShowTypeText(TaskShowType value)
        {
            var data = taskSetting.ShowTexts.Where(x => x.ShowType.Equals(value)).FirstOrDefault();
            if (data != null)
            {
                return data.TypeText;
            }
            return "";
        }

        public void UpdateTask(int index, TaskDetail TaskDetail)
        {
            EditorUtility.SetDirty(taskSetting);
            var newtem = taskSetting.Tasks;
            newtem[index] = TaskDetail;
            taskSetting.Tasks = newtem;
            //m_FoodListView.Rebuild();
        }

        private void BindTaskListItem(VisualElement item, int index)
        {
            TaskItemEditor TaskItemEditor = item as TaskItemEditor;
            TaskItemEditor.SetTaskItem(this, index, taskSetting.Tasks[index], m_TaskTypeList, m_TaskShowTypeList, m_FoodNameList, (taskSetting.Tasks[index].Id & 1) == 1 ? ListItemOddClass : ListItemEvenClass);
        }

        private VisualElement MakeTaskItem()
        {
            TaskItemEditor ve = new TaskItemEditor();
            return ve;
        }

        public override void OnPostSave()
        {
            base.OnPostSave();
        }
    }
}