using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.Data
{
    using System.Linq;
    using UnityGame.App;

    public class TaskInfo
    {
        public int m_ID;
        public int m_Progress = 0;
        public TaskStatus m_Status;

        public TaskInfo(int iD, int progress, TaskStatus status)
        {
            m_ID = iD;
            m_Progress = progress;
            m_Status = status;
        }
    }

    public static class TaskInfoExtensionMethod
    {
        public static TaskInfo FirstOrDefault(this IEnumerable<TaskInfo> source, int id)
        {
            var person = Enumerable.FirstOrDefault(source);

            if (person == null)
                return new TaskInfo(id, 0, TaskStatus.NotShow);

            return person;
        }
    }
}