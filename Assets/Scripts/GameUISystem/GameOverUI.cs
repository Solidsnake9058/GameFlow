using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityGame.App.Manager;

public class GameOverUI : IGameUISystem
{
    [SerializeField]
    private Button m_RetryGameButton = default;
    [SerializeField]
    private Animator m_Animator = default;

    public override void GameSetting()
    {
    }

    protected override void ShowEvent()
    {
        m_Animator.SetTrigger("Show");
        m_RetryGameButton.onClick.AddListener(RetryGame);
    }

    public void TransAnimaFin()
    {
        GameMediator.m_GameManager.ReplayLevel();
    }

    private void RetryGame()
    {
        GameMediator.m_GameManager.ReplayLevel();
    }
}
