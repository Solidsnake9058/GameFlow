using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityGame.Data
{
    using System.Linq;
    using System.Collections.Generic;
    using log4net.Core;
    using UnityGame.App;

    public class TaskItemEditor : VisualElement
    {
        private const string UXMLPATH = "Assets/Scripts/GameSetting/Editor/TaskItemEditor.uxml";

        private TaskSettingEditor m_TaskSettingEditor;
        private TaskDetail m_TaskDetail;
        private int m_TaskIndex;
        private Button m_RemoveButton;
        private Label m_NO;
        private TextField m_NoticeText;
        private DropdownField m_ClearType;
        //private DropdownField m_FoodId;
        private SliderInt m_ArrivalCount;
        private DropdownField m_ShowType;
        private IntegerField m_ShowByIdText;
        private TextField m_RewardText;
        //private Vector2Int m_FoodLevelRange = new Vector2Int(1, 50);
        private Vector2Int m_ArrivalRange = new Vector2Int(1, 10000);
        //private int m_FoodLevelTemp = 1;
        private string m_RewardTextString;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        public TaskItemEditor()
        {
            Init();
        }

        public void Init()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXMLPATH);
            visualTree.CloneTree(this);

            m_RemoveButton = this.Q<Button>("remove-btn");
            m_NO = this.Q<Label>("id");
            m_NoticeText = this.Q<TextField>("info-text");
            m_ClearType = this.Q<DropdownField>("task-type");
            //m_FoodId = this.Q<DropdownField>("food-id");
            m_ArrivalCount = this.Q<SliderInt>("arrival-count");
            m_ShowType = this.Q<DropdownField>("task-show-type");
            m_ShowByIdText = this.Q<IntegerField>("task-show-id");
            m_RewardText = this.Q<TextField>("reward");
        }

        public void SetTaskItem(TaskSettingEditor taskSettingEditor, int showNO, TaskDetail taskDetail, List<string> taskTypeList, List<string> taskShowTypeList, List<string> foodList, string customClass)
        {
            m_TaskSettingEditor = taskSettingEditor;
            m_TaskIndex = showNO;
            m_TaskDetail = taskDetail;
            this.AddToClassList(customClass);

            m_NO.text = $"{taskDetail.Id}";
            m_NoticeText.SetValueWithoutNotify(m_TaskDetail.TaskText);
            m_ClearType.choices = taskTypeList;
            m_ClearType.SetValueWithoutNotify(m_TaskSettingEditor.GetTaskEventTypeText(m_TaskDetail.ClearType));
            //m_FoodId.choices = foodList;
            //m_FoodId.SetValueWithoutNotify(taskSettingEditor.GetFoodNameById(taskDetail.FoodId));
            m_ShowType.choices = taskShowTypeList;
            m_ShowType.SetValueWithoutNotify(m_TaskSettingEditor.GetTaskShowTypeText(m_TaskDetail.ShowType));
            m_ShowByIdText.SetValueWithoutNotify(taskDetail.ShowById);
            m_RewardTextString = m_TaskDetail.Reward.ToString();
            m_RewardText.SetValueWithoutNotify(m_RewardTextString);
            //SetFoodLevelRange();
            ClearTypeSwitch();
            ShowTypeSwitch();

            //m_FoodLevel.SetValueWithoutNotify(bussinessLevel.FoodLevel);

            //m_Level.RegisterValueChangedCallback(AdjustBusinessLevel);
            m_NoticeText.RegisterValueChangedCallback(AdjustTaskDesc);
            m_ClearType.RegisterValueChangedCallback(AdjustClearType);
            m_ShowType.RegisterValueChangedCallback(AdjustShowType);
            m_ShowByIdText.RegisterValueChangedCallback(AdjustShowById);
            //m_FoodId.RegisterValueChangedCallback(AdjustFoodId);
            //m_ArrivalCount.RegisterValueChangedCallback(AdjustFoodLevel);
            m_RewardText.RegisterValueChangedCallback(AdjustReward);
        }

        private void AdjustTaskDesc(ChangeEvent<string> evt)
        {
            m_TaskDetail.TaskText = evt.newValue;
            m_TaskSettingEditor.UpdateTask(m_TaskIndex, m_TaskDetail);
        }

        private void AdjustClearType(ChangeEvent<string> evt)
        {
            TaskEventType id = m_TaskSettingEditor.GetTaskEventType(evt.newValue);
            if (id >= 0)
            {
                m_TaskDetail.ClearType = id;
                ClearTypeSwitch();
                m_TaskSettingEditor.UpdateTask(m_TaskIndex, m_TaskDetail);
            }
            else
            {
                m_ClearType.SetValueWithoutNotify(evt.previousValue);
            }
        }

        private void ClearTypeSwitch()
        {
            switch (m_TaskDetail.ClearType)
            {
                //case TaskEventType.UseGoldEgg:
                //case TaskEventType.UseSilverEgg:
                //case TaskEventType.RewardBonus:
                //case TaskEventType.ChickenCount:
                //case TaskEventType.BusinessLevel:
                //    m_FoodId.focusable = false;
                //    m_ArrivalCount.focusable = true;
                //    ArrivalRangeSwitch(false);
                //    break;
                //case TaskEventType.Share:
                //case TaskEventType.ReadManual:
                //case TaskEventType.ReadHelp:
                //case TaskEventType.PushOn:
                //case TaskEventType.Tutorial1:
                //case TaskEventType.Tutorial2:
                //    m_FoodId.focusable = false;
                //    m_ArrivalCount.focusable = false;
                //    break;
                //case TaskEventType.BuyFood:
                //case TaskEventType.UnlockFood:
                //    m_FoodId.focusable = true;
                //    m_ArrivalCount.focusable = true;
                //    ArrivalRangeSwitch(true);
                //    break;
            }
        }

        private void AdjustShowType(ChangeEvent<string> evt)
        {
            TaskShowType id = m_TaskSettingEditor.GetTaskShowType(evt.newValue);
            if (id >= 0)
            {
                m_TaskDetail.ShowType = id;
                ShowTypeSwitch();
                m_TaskSettingEditor.UpdateTask(m_TaskIndex, m_TaskDetail);
            }
            else
            {
                m_ClearType.SetValueWithoutNotify(evt.previousValue);
            }
        }

        private void AdjustShowById(ChangeEvent<int> evt)
        {
            if (evt.newValue > 0)
            {
                m_TaskDetail.ShowById = evt.newValue;
                m_TaskSettingEditor.UpdateTask(m_TaskIndex, m_TaskDetail);
            }
            else
            {
                m_ShowByIdText.SetValueWithoutNotify(evt.previousValue);
            }
        }

        private void ShowTypeSwitch()
        {
            switch (m_TaskDetail.ShowType)
            {
                case TaskShowType.WhenStart:
                    m_ShowByIdText.focusable = false;
                    break;
                case TaskShowType.WhenClear:
                    m_ShowByIdText.focusable = true;
                    break;
            }
        }

        private void ArrivalRangeSwitch(bool isFood)
        {
            //if (isFood)
            //{
            //    m_ArrivalCount.highValue = m_FoodLevelRange.y;
            //    m_ArrivalCount.lowValue = m_FoodLevelRange.x;
            //}
            //else
            {
                m_ArrivalCount.highValue = m_ArrivalRange.y;
                m_ArrivalCount.lowValue = m_ArrivalRange.x;
            }
        }

        //private void AdjustFoodId(ChangeEvent<string> evt)
        //{
        //    int id = m_TaskSettingEditor.GetFoodIdByName(evt.newValue);
        //    if (id > 0)
        //    {
        //        m_TaskDetail.FoodId = id;
        //        //SetFoodLevelRange();
        //        m_TaskSettingEditor.UpdateTask(m_TaskIndex, m_TaskDetail);
        //    }
        //    else
        //    {
        //        m_FoodId.SetValueWithoutNotify(m_TaskSettingEditor.GetFoodNameById(m_TaskDetail.FoodId));
        //        //m_TaskSettingEditor.UpdateBusinessLevel(m_TaskIndex, m_TaskDetail);
        //    }
        //}

        //private void AdjustFoodLevel(ChangeEvent<int> evt)
        //{
        //    m_TaskDetail.Goal = evt.newValue;
        //    m_TaskSettingEditor.UpdateTask(m_TaskIndex, m_TaskDetail);
        //}

        //private void SetFoodLevelRange()
        //{
        //    int max = m_TaskSettingEditor.GetFoodDetailMaxLevel(m_TaskDetail.FoodId);
        //    if (max > 0)
        //    {
        //        m_FoodLevelRange.y = max;
        //        m_FoodLevelRange.x = 1;
        //        //m_TaskDetail.Goal = Mathf.Clamp(m_TaskDetail.Goal, 1, max);
        //    }
        //    else
        //    {
        //        m_FoodLevelRange.y = 1;
        //        m_FoodLevelRange.x = 1;
        //        //m_TaskDetail.Goal = 1;
        //    }
        //    m_ArrivalCount.SetValueWithoutNotify(m_TaskDetail.Goal);
        //}

        private void AdjustReward(ChangeEvent<string> val)
        {
            if (!m_RewardTextString.Equals(val))
            {
                CustomInteger newVal = new CustomInteger(val.newValue);
                if (newVal > 0)
                {
                    m_TaskDetail.RewardText = newVal.ToString();
                    m_RewardTextString = m_TaskDetail.RewardText;
                    //m_FoodItemEditor.UpdateDetail(m_DetailIndex, m_FoodDetail);
                }
                m_RewardText.SetValueWithoutNotify(m_RewardTextString);
                m_TaskSettingEditor.UpdateTask(m_TaskIndex, m_TaskDetail);
            }
        }

        public class Factory : UxmlFactory<TaskItemEditor, Traits> { }

        public class Traits : UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
    }
}