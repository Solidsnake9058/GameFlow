using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGame.Data;

namespace UnityGame.App.Manager
{
    public class TaskSystem : IGameItem
    {
        [SerializeField]
        private TaskSetting _TaskSetting = default;
        private TaskRule m_TaskRules;
        private List<TaskItem> m_TaskItems;
        private static TaskItem _taskItem = null;
        public List<TaskItem> m_TaskItemsList { get { return m_TaskItems; } }

        private const string m_DataName = "AchievementsData";

        public override void Initialize()
        {
            m_TaskRules = new TaskRule(this);
            m_TaskItems = new List<TaskItem>();

            Debug.Log(_TaskSetting.Tasks.Count);
            if (_TaskSetting is null)
            {
                return;
            }

            foreach (var item in _TaskSetting.Tasks)
            {
                m_TaskItems.Add(item.ConvterToTaskItem());
            }
            LoadTasks();
        }

        private void OnDestroy()
        {
            m_TaskRules?.StopListening(); ;
        }

        public TaskItem GetTaskById(int id)
        {
            var item = m_TaskItemsList.Where(x => x.m_TaskID.Equals(id)).FirstOrDefault();
            return item;
        }

        /// <summary>
		/// Unlocks the specified achievement (if found).
		/// </summary>
		/// <param name="taskID">Achievement I.</param>
		public void UnlockTask(int taskID)
        {
            _taskItem = TaskManagerContains(taskID);
            if (_taskItem != null)
            {
                _taskItem.UnlockTask();
            }
        }

        /// <summary>
        /// Locks the specified achievement (if found).
        /// </summary>
        /// <param name="achievementID">Achievement ID.</param>
        //public void LockAchievement(int achievementID)
        //      {
        //          _taskItem = TaskManagerContains(achievementID);
        //          if (_taskItem != null)
        //          {
        //              _taskItem.LockTask();
        //          }
        //      }

        /// <summary>
        /// Adds progress to the specified achievement (if found).
        /// </summary>
        /// <param name="taskID">Achievement ID.</param>
        /// <param name="newProgress">New progress.</param>
        public void AddProgress(int taskID, int newProgress)
        {
            _taskItem = TaskManagerContains(taskID);
            if (_taskItem != null)
            {
                _taskItem.AddProgress(newProgress);
                SaveTasks();
                //GameMediator.RefreshTaskUI();
            }
        }

        /// <summary>
		/// Sets the progress of the specified achievement (if found) to the specified progress.
		/// </summary>
		/// <param name="taskID">Achievement ID.</param>
		/// <param name="newProgress">New progress.</param>
		public void SetProgress(int taskID, int newProgress)
        {
            _taskItem = TaskManagerContains(taskID);
            if (_taskItem != null)
            {
                _taskItem.SetProgress(newProgress);
                SaveTasks();
                //GameMediator.RefreshTaskUI();
            }
        }

        public void SetTaskRewarded(int taskID)
        {
            _taskItem = TaskManagerContains(taskID);
            if (_taskItem != null)
            {
                _taskItem.m_Status = TaskStatus.Rewarded;
                SaveTasks();
                //GameMediator.RefreshTaskUI();
            }
        }

        public bool GetCanTaskReward(int taskID)
        {
            _taskItem = TaskManagerContains(taskID);
            if (_taskItem != null)
            {
                return _taskItem.m_Status == TaskStatus.Complete;
            }
            return false;
        }

        /// <summary>
        /// Determines if the achievement manager contains an achievement of the specified ID. Returns it if found, otherwise returns null
        /// </summary>
        /// <returns>The achievement corresponding to the searched ID if found, otherwise null.</returns>
        /// <param name="searchedID">Searched I.</param>
        private TaskItem TaskManagerContains(int searchedID)
        {
            if (m_TaskItems.Count == 0)
            {
                return null;
            }
            return m_TaskItems.Where(x => x.m_TaskID == searchedID).FirstOrDefault();
        }


        // SAVE ------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes saved data and resets all achievements from a list
        /// </summary>
        /// <param name="listID">The ID of the achievement list to reset.</param>
        public void ResetTasks()
        {
            if (m_TaskItems != null)
            {
                foreach (var achievement in m_TaskItems)
                {
                    achievement.m_ProgressCurrent = 0;
                    achievement.m_Status = TaskStatus.NotShow;
                }
            }

            //ManagerMediator.m_SaveManager.ClearData(m_DataName);
            SaveManager.UpdateTaskInfos(null);
            SaveManager.Save();
            LoadTasks();
            Debug.LogFormat("Task Reset");
        }

        /// <summary>
		/// Loads the saved achievements file and updates the array with its content.
		/// </summary>
		public void LoadTasks()
        {
            var data = SaveManager.TaskInfos();
            ExtractTaskData(data);

            foreach (var item in m_TaskItems)
            {
                if (item.m_Status <= TaskStatus.Process)
                {
                    m_TaskRules.AddGameEvent(item.m_GameEventName, item.m_TaskID);
                }
                if (item.m_Status <= TaskStatus.NotShow)
                {
                    //var tgId = m_TaskItems.Where(x => x.m_TaskID == item.m_UnlockContitionId).FirstOrDefault();
                    //if (tgId != null && tgId.m_Status >= TaskStatus.Complete)
                    //{
                    //    item.m_Status = TaskStatus.Process;
                    //}
                    //else
                    {
                        m_TaskRules.TaskUnlockList(item.m_TaskID, item.m_UnlockContitionId);
                    }
                }
            }
            foreach (var item in m_TaskItems)
            {
                item.EvaluateProgress();
            }
        }

        /// <summary>
		/// Saves the achievements current status to a file on disk
		/// </summary>
		public void SaveTasks()
        {
            TaskData data = new TaskData();
            FillTaskData(data);
            SaveManager.UpdateTaskInfos(data.tasks.ToList());
            SaveManager.Save();
        }


        /// <summary>
		/// Serializes the contents of the achievements array to a serialized, ready to save object
		/// </summary>
		/// <param name="serializedInventory">Serialized inventory.</param>
		private void FillTaskData(TaskData taskDatas)
        {
            taskDatas.tasks = new TaskInfo[m_TaskItems.Count];

            for (int i = 0; i < m_TaskItems.Count; i++)
            {
                TaskInfo newTask = new TaskInfo(m_TaskItems[i].m_TaskID, m_TaskItems[i].m_ProgressCurrent, m_TaskItems[i].m_Status);
                taskDatas.tasks[i] = newTask;
            }
        }

        /// <summary>
		/// Extracts the serialized achievements into our achievements array if the achievements ID match.
		/// </summary>
		/// <param name="missionInfos">Serialized achievements.</param>
		public void ExtractTaskData(List<TaskInfo> missionInfos)
        {
            if (missionInfos == null)
            {
                return;
            }

            for (int i = 0; i < m_TaskItems.Count; i++)
            {
                var info = missionInfos.Where(x => x.m_ID == m_TaskItems[i].m_TaskID).FirstOrDefault();
                if (info != null)
                {
                    m_TaskItems[i].m_Status = info.m_Status;
                    m_TaskItems[i].m_ProgressCurrent = info.m_Progress;
                }
            }
        }
    }


    /// <summary>
	/// A serializable class used to store an achievement into a save file
	/// </summary>
    //[Serializable]
    //public class SerializedAchievement
    //{
    //    public string m_AchievementID;
    //    public bool m_UnlockedStatus;
    //    public int m_ProgressCurrent;

    //    /// <param name="achievementID">Achievement I.</param>
    //    /// <param name="unlockedStatus">If set to <c>true</c> unlocked status.</param>
    //    /// <param name="progressCurrent">Progress current.</param>
    //    public SerializedAchievement(string achievementID, bool unlockedStatus, int progressCurrent)
    //    {
    //        m_AchievementID = achievementID;
    //        m_UnlockedStatus = unlockedStatus;
    //        m_ProgressCurrent = progressCurrent;
    //    }
    //}


    [Serializable]
    /// <summary>
    /// Serializable achievement data.
    /// </summary>
    public class TaskData
    {
        public TaskInfo[] tasks;
    }
}