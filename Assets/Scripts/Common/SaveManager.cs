using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySingleton;
using System.Linq;
using System;

public class SaveManager : MonoBehaviourProtectedSingleton<SaveManager>
{
#region BaseFunction
    private static PlayerSaveData GameData = null;

    protected override void Awake()
    {
        base.Awake();
        Load();
    }

    public static bool IsNull()
    {
        return (GameData == null);
    }

    public static void Load()
    {
        GameData = null;
        GameData = PlayerPrefsX.GetClassDecrypt("PlayerSaveData", new PlayerSaveData());
    }

    public static void Save()
    {
        PlayerPrefsX.SetClassEncrypt("PlayerSaveData", GameData);
    }

    public static void Clear()
    {
        Debug.Log("Clear Save Data");
        PlayerPrefsX.DeleteAllKey();
        Load();
    }

    public static void UnlockAllStage()
    {
        //GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        //if (gm != null)
        //{
        //    List<StageSetting> stages = gm.GetStageManager().GetStageSettings();
        //    for (int i = 0; i < stages.Count; i++)
        //    {
        //        if (!GameData.m_StageData.ContainsKey(stages[i].m_StageIndex))
        //        {
        //            GameData.m_StageData.Add(stages[i].m_StageIndex, new StageAssets(StageAssets.StageState.NotClear));
        //        }
        //    }
        //    Save();
        //}
    }

    #endregion

    #region GameControlFunction
    //Stage
    public static int DisplayStage => GameData.m_DisplayStage;

    public static void SetCurrentStageNo(int no) => GameData.m_CurrentStageNo = no;
    public static int CurrentStageNo => GameData.m_CurrentStageNo;
    public static void CreaseDisplayStage() => GameData.m_DisplayStage++;
    //bonus
    public static int BonusStageCount => GameData.m_BonusStageCount;
    public static void SetBonusStageCountNext() => GameData.m_BonusStageCount++;
    public static void ResetBonusStageCount() => GameData.m_BonusStageCount = 0;
    public static int BonusStageIndex => Mathf.Max(GameData.m_BonusStageIndex, 1);
    public static void SetBonusStageIndex(int index) => GameData.m_BonusStageIndex = index;
    public static bool IsBonusStage => GameData.m_IsBonusOn;
    public static void SetIsBonusStage(bool isOn) => GameData.m_IsBonusOn = isOn;
    //Player data
    public static bool IsPurchasedNoAds() => Convert.ToBoolean(GameData.PlayerAssets.PlayerIsBuyNoAds);
    public static void PurchasedNoAds() => GameData.PlayerAssets.PlayerIsBuyNoAds = 1;
    public static void AddPlayerGamingTimes() => GameData.PlayerAssets.PlayerGamingTimes++;


    public static void SetStageClear(int index, StageAssets.StageState state)
    {
        StageAssets.StageState temp = (StageAssets.StageState)Mathf.Min((int)Enum.GetNames(typeof(StageAssets.StageState)).Length - 1, (int)state);
        if (GameData.m_StageData.ContainsKey(index))
        {
            GameData.m_StageData[index].m_StageState = (StageAssets.StageState)Mathf.Max((int)GameData.m_StageData[index].m_StageState, (int)temp);
        }
        else
        {
            GameData.m_StageData.Add(index, new StageAssets(temp));
            GameData.m_StageData.Add(index + 1, new StageAssets(StageAssets.StageState.NotClear));
        }
    }

    public static bool GetIsStagePlayable(int index)
    {
        if (GameData.m_StageData.ContainsKey(index))
        {
            return true;
        }
        return false;
    }

    public static StageAssets.StageState GetStageState(int index)
    {
        if (GameData.m_StageData.ContainsKey(index))
        {
            return GameData.m_StageData[index].m_StageState;
        }
        return StageAssets.StageState.NoData;
    }

    public static bool IsStageCleared(int index)
    {
        return GetStageState(index).Equals(StageAssets.StageState.Clear);
    }

    public static bool GetIsAllStageClear(int maxStageIndex)
    {
        if (GameData.m_StageData.ContainsKey(maxStageIndex))
        {
            return GameData.m_StageData[maxStageIndex].m_StageState.Equals(StageAssets.StageState.Clear);
        }
        return false;
    }

    public static int VideoAdIndex => GameData.m_VideoAdIndex;

    public static void SetVideoAdIndex(int value) => GameData.m_VideoAdIndex = value;

    #endregion

    public static int PlayerGamingTimes => GameData.PlayerAssets.PlayerGamingTimes;
}
