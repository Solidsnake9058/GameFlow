using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityGame.Data;

public class PlayerSaveData
{
    public PlayerAssets PlayerAssets = new PlayerAssets();

    private static readonly int[] m_DefaultStageStyle = { };

    public int m_CurrentStageNo;
    public int m_DisplayStage;
    public int m_BonusStageCount;
    public int m_BonusStage;
    public bool m_IsBonusOn = false;
    public List<TaskInfo> m_TaskInfos;
    public SortedDictionary<int, StageAssets> m_StageData = new SortedDictionary<int, StageAssets>();
    public HashSet<int> m_UnlockStageStyle = new HashSet<int>();
    public DateTime m_FirstPlayTime = DateTime.Now;
    public int m_VideoAdIndex;

    public PlayerSaveData()
    {
        m_DisplayStage = 1;
        m_CurrentStageNo = 0;
        m_BonusStageCount = 0;
        m_BonusStage = 0;

        m_VideoAdIndex = 0;

        m_StageData.Add(m_CurrentStageNo, new StageAssets(StageAssets.StageState.NotClear));

        SetDefaultStageStyle();
    }

    public void SetDefaultStageStyle()
    {
        m_UnlockStageStyle = new HashSet<int>();
        for (int i = 0; i < m_DefaultStageStyle.Length; i++)
        {
            m_UnlockStageStyle.Add(m_DefaultStageStyle[i]);
        }
    }
}

public class PlayerAssets
{
    [JsonProperty]
    private int _playerIsBuyNoAds;

    [JsonProperty]
    private int _playerGamingTimes;

    public PlayerAssets()
    {
        PlayerIsBuyNoAds = 0;
        _playerGamingTimes = 0;

    }

    [JsonIgnore]
    public int PlayerIsBuyNoAds
    {
        get { return PlayerPrefsX.FastUnMask(_playerIsBuyNoAds); }
        set { _playerIsBuyNoAds = PlayerPrefsX.FastMask(value); }
    }

    [JsonIgnore]
    public int PlayerGamingTimes
    {
        get { return PlayerPrefsX.FastUnMask(_playerGamingTimes); }
        set { _playerGamingTimes = PlayerPrefsX.FastMask(value); }
    }
}

public class StageAssets
{
    public StageAssets(StageState state)
    {
        m_StageState = state;
        if ((int)state >= 0)
        {
            m_PlayedTimes = 1;
        }
        if (state >= StageState.Clear)
        {
            m_ClearedTimes = 1;
        }
    }

    [JsonProperty]
    public StageState m_StageState;
    public int m_PlayedTimes;
    public int m_ClearedTimes;

    public enum StageState
    {
        NoData = -1,
        NotClear = 0,
        Clear = 99,
        //AllStar = 100,
    }
}
