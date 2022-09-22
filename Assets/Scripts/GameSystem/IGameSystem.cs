using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.App.Manager;

public abstract class IGameSystem
{
    protected GameManager m_GameManager = null;
    public IGameSystem(GameManager gameManager)
    {
        m_GameManager = gameManager;
    } 

    public abstract void Initialize();
    public virtual void SystemUpdate() { }
    public virtual void GameSetting() { }

    public abstract void Pause();
    public abstract void Resume();
}
