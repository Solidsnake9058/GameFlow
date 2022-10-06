using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityGame.App.Manager;

public abstract class IGameItem : MonoBehaviour
{
    private Action m_ReleaseAction;

    public virtual void Initialize()
    {
    }
    public virtual void SystemUpdate() { }
    public virtual void SystemFixedUpdate() { }
    public virtual void SystemLateUpdate() { }
    public virtual void GameSetting() { }

    public virtual void Pause() { }
    public virtual void Resume() { }

    private AsyncOperationHandle? m_handle;
    public AsyncOperationHandle Handle => m_handle.Value;

    public void SetAsyncOperationHandle(AsyncOperationHandle handle)
    {
        m_handle = handle;
    }

    private void OnDestroy()
    {
        m_ReleaseAction?.Invoke();
    }

    public void SetOnDestroy(Action action)
    {
        m_ReleaseAction = action;
    }

}