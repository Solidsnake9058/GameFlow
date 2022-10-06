using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.Data
{
    //using DG.Tweening;

    [CreateAssetMenu(fileName = "GameSetting", menuName = "ScriptableObject/GameSetting")]
    public class GameSetting : SettingBase
    {
        public SystemData System = default;



        [Serializable]
        public class SystemData
        {
            public float StaminaBase;
            public float StaminaIncrease;
            public Color StaminaColorMax;
            public Color StaminaColorMin;
            public float StaminaReduce;
            public float SpeedGangIncrease;
            public float SpeedGangReduce;
            public Vector2 JumpSpeed;
            public float CalorieReduceBase;
            public float WeightMin;
            public float WeightNormal;
            public float WeightMax;
            public float FatRate => (WeightMax - WeightNormal) / (WeightMax - WeightMin);
            public float ClearResultMin;
            public float ClearResultMax;
            public float ResultGangSpeed;
            public float ResultDelayTime;
            public int SweatEachPoint;
            public SweatCount[] SweatCountRate;
            public int JumpRopeCoinBase;
            public float CoinJumpForce;
            public float CoinJumpTime;
            public float CoinJumpFinalScale;
            //public Ease CoinJumpEase;
        }

        [Serializable]
        public class SweatCount
        {
            public float StaminaRate;
            public int PointCount;
        }

//#if UNITY_EDITOR
//        public override string GetEditorName()
//        {
//            return "GameSettingEditor";
//        }
//#endif
    }
}