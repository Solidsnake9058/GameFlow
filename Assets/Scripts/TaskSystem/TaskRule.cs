using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.App;
using UnityGame.App.Manager;

namespace UnityGame.App
{
    /// <summary>
    /// That class is meant to be extended to implement the achievement rules specific to your game.
    /// </summary>
    public class TaskRule : EventListener<GameEvent>, EventListener<TaskUnlockedEvent>
    {
        private TaskSystem m_TaskSystem;
        private Dictionary<string, List<int>> m_GameEventList = new Dictionary<string, List<int>>();
        private Dictionary<int, List<int>> m_TaskUnlockList = new Dictionary<int, List<int>>();

        public TaskRule(TaskSystem taskSystem)
        {
            m_TaskSystem = taskSystem;
            this.EventStartListening<TaskUnlockedEvent>();
            this.EventStartListening<GameEvent>();
        }

        public void StopListening()
        {
            this.EventStopListening<TaskUnlockedEvent>();
            this.EventStopListening<GameEvent>();
        }

        public virtual void OnEvent(TaskUnlockedEvent taskEvent)
        {
            if (m_TaskSystem != null && taskEvent.m_TaskItem != null)
            {
                if (m_TaskUnlockList.ContainsKey(taskEvent.m_TaskItem.m_TaskID))
                {
                    var list = m_TaskUnlockList[taskEvent.m_TaskItem.m_TaskID];
                    foreach (var item in list)
                    {
                        m_TaskSystem.UnlockTask(item);
                    }
                }
                m_TaskSystem.SaveTasks();
            }
        }

        //examples implement EventListener<GameEvent> ................................
        public virtual void OnEvent(GameEvent gameEvent)
        {
            //if (gameEvent.EventName == "GetMoney")
            //{
            //    m_TaskSystem.AddProgress(1, 1);
            //}

            if (m_GameEventList.ContainsKey(gameEvent.EventName))
            {
                var list=m_GameEventList[gameEvent.EventName];
                for (int i = 0; i < list.Count; i++)
                {
                    m_TaskSystem.SetProgress(list[i], gameEvent.Value);
                }
            }
        }

        public void AddGameEvent(string eventName, int id)
        {
            if (m_GameEventList == null)
            {
                m_GameEventList = new Dictionary<string, List<int>>();
            }
            if (!m_GameEventList.ContainsKey(eventName))
            {
                m_GameEventList.Add(eventName, new List<int>());
            }
            m_GameEventList[eventName].Remove(id);
            m_GameEventList[eventName].Add(id);
        }

        public void RemoveGameEvent(string eventName, int id)
        {
            if (m_GameEventList == null)
            {
                m_GameEventList = new Dictionary<string, List<int>>();
            }
            if (m_GameEventList.ContainsKey(eventName))
            {
                m_GameEventList[eventName].Remove(id);
            }
        }

        public void TaskUnlockList(int unlockId,int clearId)
        {
            if (m_TaskUnlockList == null)
            {
                m_TaskUnlockList = new Dictionary<int, List<int>>();
            }
            if (!m_TaskUnlockList.ContainsKey(clearId))
            {
                m_TaskUnlockList.Add(clearId, new List<int>());
            }
            m_TaskUnlockList[clearId].Remove(unlockId);
            m_TaskUnlockList[clearId].Add(unlockId);
        }

        //examples implement EventListener<CheckPointEvent> ................................
        //public virtual void OnEvent(CheckPointEvent checkPointEvent)
        //{
        //	if (checkPointEvent.Order > 0)
        //	{
        //		MMAchievementManager.UnlockAchievement("SteppingStone");
        //	}
        //}

    }
}