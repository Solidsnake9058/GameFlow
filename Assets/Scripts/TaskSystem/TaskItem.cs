using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.App.Manager;

namespace UnityGame.App
{
    /// <summary>
	/// This achievement system supports 2 types of achievements : simple (do something > get achievement), and progress based (jump X times, kill X enemies, etc).
	/// </summary>
	public enum TaskType { Simple, Progress }

    [Serializable]
    public class TaskItem
    {
        [Header("Identification")]
        /// the unique identifier for this achievement
        public int m_TaskID;
        /// is this achievement progress based or 
        public TaskType m_TaskType;
        /// if this is true, the achievement won't be displayed in a list
        public bool m_HiddenTask;
        /// if this is true, the achievement has been unlocked. Otherwise, it's still up for grabs
        public TaskEventType m_EventType;
        public bool m_UnlockedStatus => m_Status.Equals(TaskStatus.Process) || m_Status.Equals(TaskStatus.Complete);
        public bool m_IsComplete => m_Status.Equals(TaskStatus.Complete);
        public bool m_IsRewarded => m_Status.Equals(TaskStatus.Rewarded);
        public TaskStatus m_Status;
        public int m_UnlockContitionId;
        public string m_GameEventName;// => $"{m_EventType}_{m_ProgressFoodId:0000}";

        [Header("Description")]
        /// the achievement's name/title
        public string m_Title;
        /// the achievement's description
        public string m_Description;
        /// the amount of points unlocking this achievement gets you
        public int m_Points;

        //[Header("Image and Sounds")]
        ///// the image to display while this achievement is locked
        //public Sprite m_LockedImage;
        ///// the image to display when the achievement is unlocked
        //public Sprite m_UnlockedImage;
        ///// a sound to play when the achievement is unlocked
        //public AudioClip m_UnlockedSound;

        [Header("Progress")]
        /// the amount of progress needed to unlock this achievement.
        public int m_ProgressTarget;
        /// the current amount of progress made on this achievement
        public int m_ProgressCurrent;
        public int m_ProgressFoodId;

        //protected AchievementDisplayItem _achievementDisplayItem;

        public virtual void UnlockTask()
        {
            // if the achievement has already been unlocked, we do nothing and exit
            if (m_Status.Equals(TaskStatus.NotShow))
            {
                m_Status = TaskStatus.Process;
            }
            EvaluateProgress();
        }

        /// <summary>
        /// Unlocks the achievement, asks for a save of the current achievements, and triggers an AchievementUnlockedEvent for this achievement.
        /// This will usually then be caught by the AchievementDisplay class.
        /// </summary>
        public virtual void FinishTask()
        {
            // if the achievement has already been unlocked, we do nothing and exit
            if (m_Status.Equals(TaskStatus.NotShow) || m_Status.Equals(TaskStatus.Rewarded))
            {
                return;
            }

            m_Status = TaskStatus.Complete;

            TaskUnlockedEvent.Trigger(this);
        }

        /// <summary>
        /// Locks the achievement.
        /// </summary>
        //public virtual void LockTask()
        //{
        //    m_UnlockedStatus = false;
        //}
        /// <summary>
        /// Adds the specified value to the current progress.
        /// </summary>
        /// <param name="newProgress">New progress.</param>
        public virtual void AddProgress(int newProgress)
        {
            m_ProgressCurrent += newProgress;
            EvaluateProgress();
        }

        /// <summary>
        /// Sets the progress to the value passed in parameter.
        /// </summary>
        /// <param name="newProgress">New progress.</param>
        public virtual void SetProgress(int newProgress)
        {
            m_ProgressCurrent = Mathf.Max(newProgress, m_ProgressCurrent);
            EvaluateProgress();
        }

        /// <summary>
        /// Evaluates the current progress of the achievement, and unlocks it if needed.
        /// </summary>
        public virtual void EvaluateProgress()
        {
            if (m_ProgressCurrent >= m_ProgressTarget)
            {
                m_ProgressCurrent = m_ProgressTarget;
                FinishTask();
            }
        }

        /// <summary>
        /// Copies this achievement (useful when loading from a scriptable object list)
        /// </summary>
        public virtual TaskItem Copy()
        {
            TaskItem clone = new TaskItem();
            // we use Json utility to store a copy of our achievement, not a reference
            clone = JsonUtility.FromJson<TaskItem>(JsonUtility.ToJson(this));
            return clone;
        }
    }

    /// <summary>
    /// An event type used to broadcast the fact that an achievement has been unlocked
    /// </summary>
    public struct TaskUnlockedEvent
    {
        /// the achievement that has been unlocked
        public TaskItem m_TaskItem;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="newTask">New achievement.</param>
        public TaskUnlockedEvent(TaskItem newTask)
        {
            m_TaskItem = newTask;
        }

        static TaskUnlockedEvent e;
        public static void Trigger(TaskItem newTask)
        {
            e.m_TaskItem = newTask;
            EventManager.TriggerEvent(e);
        }
    }
}