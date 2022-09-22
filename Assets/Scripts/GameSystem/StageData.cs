using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class StageData : IGameItem
{
    public StageData Clone()
    {
        StageData stageData = new StageData()
        {
        };

        return stageData;
    }

#if TEST_MODE
    private void Start()
    {
        GameSetting();
    }

    private void Update()
    {
        SystemUpdate();
    }
#endif

    
}