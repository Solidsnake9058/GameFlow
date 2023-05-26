#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace UnityGame.App.Manager
{
    public class CreatorStageManager : StageManager
    {
        public override bool LoadStage(ref int stageNo, ref int failStage)
        {
            return SetStageDate(CreatorMediator.m_CreateShapeManager.m_TestPlayStageData);
        }

        public void ClearStage()
        {
        }
    }
}
#endif