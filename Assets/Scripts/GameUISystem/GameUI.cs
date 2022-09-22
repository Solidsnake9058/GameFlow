using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class GameUI : IGameUISystem
{
    [SerializeField]
    private GameObject m_UIElement = default;

    [SerializeField]
    private RectTransform m_StageBar = default;
    private Vector2 m_StageBarSizeBase;
    private Vector2 m_StageBarSize;
    [SerializeField]
    private TextMeshProUGUI m_StageTitleText = default;
    [SerializeField]
    private TextMeshProUGUI m_NextStageTitleText = default;
    [SerializeField]
    private Button m_ClearDataButton = default;

    public override void GameSetting()
    {
#if Debug
        m_ClearDataButton.gameObject.SetActive(true);
        m_ClearDataButton.onClick.AddListener(()=> { SaveManager.Clear();m_GameManager.ReplayStart(); });
#else
        m_ClearDataButton.gameObject.SetActive(false);
#endif
        m_StageBarSize = m_StageBarSizeBase = m_StageBar.sizeDelta;
        m_StageBarSize.x = 0;
        m_StageBar.sizeDelta = m_StageBarSize;
        //EnvironmentManager.AddMuteButton(m_MuteButton);
    }

    public void SetStageBar(float rate)
    {
        m_StageBarSize.x = m_StageBarSizeBase.x * rate;
        m_StageBar.sizeDelta = m_StageBarSize;
    }

    public void SetStageTitle(int index)
    {
        m_StageTitleText.SetText($"{index}");
        m_NextStageTitleText.SetText($"{index + 1}");
    }

    public void SetUIDisplay(bool isShow)
    {
        m_UIElement.SetActive(isShow);
    }

    protected override void HideEvent()
    {
        StopAllCoroutines();
    }
}