using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IGameUISystem : IGameItem
{
    public Transform m_UIBase;
    public bool m_IsShow { get { return m_UIBase ?? m_UIBase.gameObject.activeSelf; } }

    public override void Pause()
    {
    }

    public override void Resume()
    {
    }

    protected virtual void ShowEvent() { }
    protected virtual void HideEvent() { }

    public void ShowUI()
    {
        m_UIBase?.gameObject.SetActive(true);
        ShowEvent();
    }

    public void HideUI()
    {
        m_UIBase?.gameObject.SetActive(false);
        HideEvent();
    }
}