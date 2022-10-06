using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGame.App.Manager;

public class MainUI : IGameUISystem
{
    [SerializeField]
    private Button m_StartButton = default;
    [SerializeField]
    private StartPress m_StartPress = default;

    public override void GameSetting()
    {
        //m_StartButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        //GameMediator.m_GameManager.StartGame();
    }
}