using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.App;
using System.Linq;
using UnityGame.App.UI;

namespace UnityGame.Data
{
    [CreateAssetMenu(fileName = "TaskSetting", menuName = "ScriptableObject/TaskSetting")]
    public class TaskSetting : SettingBase
    {
        public List<TaskDetail> Tasks;
        public List<TaskEventText> EventTexts;
        public List<TaskShowText> ShowTexts;
        public List<TaskEventType> TriggerTasks;

        public Dictionary<string, TaskEventType> GetTaskTypeList()
        {
            Dictionary<string, TaskEventType> dicTaskType = new Dictionary<string, TaskEventType>();
            if (EventTexts != null)
            {
                for (int i = 0; i < EventTexts.Count; i++)
                {
                    var eventType = EventTexts[i];
                    dicTaskType.Add(eventType.TypeText, eventType.EventType);
                }
            }
            return dicTaskType;
        }

        public Dictionary<string, TaskShowType> GetTaskShowTypeList()
        {
            Dictionary<string, TaskShowType> dicTaskType = new Dictionary<string, TaskShowType>();
            if (EventTexts != null)
            {
                for (int i = 0; i < ShowTexts.Count; i++)
                {
                    var eventType = ShowTexts[i];
                    dicTaskType.Add(eventType.TypeText, eventType.ShowType);
                }
            }
            return dicTaskType;
        }

        public TaskDetail GetTaskByID(int id)
        {
            return Tasks.Where(x => x.Id == id).FirstOrDefault();
        }

        public int GetEmptyTaskId()
        {
            var sort = Tasks.OrderBy(x => x.Id).ToList();
            int id = sort.Count + 1;
            for (int i = 0; i < sort.Count; i++)
            {
                if (i + 1 != sort[i].Id)
                {
                    id = i + 1;
                    break;
                }
            }
            return id;
        }

        public static string GetTaskEventName(TaskEventType eventType, int foodId)
        {
            return TaskDetail.GetTaskEventName(eventType, foodId);
        }
    }

    [Serializable]
    public class TaskDetail
    {
        public int Id;
        public string TaskText;
        public TaskEventType ClearType;
        public int FoodId;
        public int Goal = 1;
        public TaskShowType ShowType;
        public int ShowById;
        [SerializeField]
        private string _RewardText;
        public string RewardText
        {
            get { return _RewardText; }
            set
            {
                _RewardText = value;
                _Reward = new CustomInteger(_RewardText);
            }
        }
        private CustomInteger _Reward;

        public TaskDetail(int id)
        {
            Id = id;
            TaskText = "説明";
            ClearType = TaskEventType.TouchClick;
            FoodId = 1;
            Goal = 1;
            ShowType = TaskShowType.WhenStart;
            ShowById = 1;
            _RewardText = "1000";
        }

        public CustomInteger Reward
        {
            get
            {
                if (_Reward == null || _Reward.m_Value == new CustomInteger(0))
                {
                    _Reward = new CustomInteger(RewardText);
                }
                return _Reward;
            }
        }

        public TaskItem ConvterToTaskItem()
        {
            TaskItem taskItem = new TaskItem();
            taskItem.m_TaskID = Id;
            taskItem.m_EventType = ClearType;
            taskItem.m_Status = ShowType.Equals(TaskShowType.WhenStart) ? TaskStatus.Process : TaskStatus.NotShow;
            taskItem.m_ProgressTarget = Goal;
            taskItem.m_ProgressCurrent = 0;
            //taskItem.m_ProgressFoodId = (ClearType.Equals(TaskEventType.BuyFood) || ClearType.Equals(TaskEventType.UnlockFood)) ? FoodId : 0;
            taskItem.m_Title = TaskText;
            taskItem.m_UnlockContitionId = ShowById;
            taskItem.m_GameEventName = GetTaskEventName(ClearType, FoodId);
            return taskItem;
        }

        public static string GetTaskEventName(TaskEventType eventType, int foodId)
        {
            //foodId = (eventType.Equals(TaskEventType.BuyFood) || eventType.Equals(TaskEventType.UnlockFood)) ? foodId : 0;
            return $"{eventType}_{foodId:0000}";
        }
    }

    public static class TaskDetailExtensionMethod
    {
        public static TaskDetail FirstOrDefault(this IEnumerable<TaskDetail> source)
        {
            var person = Enumerable.FirstOrDefault(source);

            if (person == null)
                return new TaskDetail(-1);

            return person;
        }
    }

    [Serializable]
    public class TaskEventText
    {
        public TaskEventType EventType;
        public string TypeText;
    }

    [Serializable]
    public class TaskShowText
    {
        public TaskShowType ShowType;
        public string TypeText;
    }
}