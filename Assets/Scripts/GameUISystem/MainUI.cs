using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : IGameUISystem
{
    [SerializeField]
    private Button m_StartButton = default;
    [SerializeField]
    private StartPress m_StartPress = default;

    public override void GameSetting()
    {
        m_StartPress.Initialize(m_GameManager);
        m_StartButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        m_GameManager.StartGame();
    }
}