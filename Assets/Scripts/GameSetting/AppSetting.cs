using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityGame.Data
{
    [CreateAssetMenu(fileName = "AppSetting", menuName = "ScriptableObject/AppSetting")]
    public class AppSetting : SettingBase
    {
        public AdData AD = default;
        public AnalyticsData Analytics = default;

        [Serializable]
        public class AdData
        {
            public int AdInterstitialStartStageNo = 3;
            public int AdInterstitialInterval = 2;
        }

        [Serializable]
        public class AnalyticsData
        {
            public List<int> TenjinClearEventStages = default;

        }
    }
}
