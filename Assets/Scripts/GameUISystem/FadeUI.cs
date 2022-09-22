using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : IGameUISystem
{
    [SerializeField]
    private Animator m_Animator = default;

    private bool _IsInBlack = false;
    public bool m_IsInBlack { get { return _IsInBlack; } }

    public void FadeOutFinish()
    {
        _IsInBlack = true;
    }

    public void FadeInFinish()
    {
        _IsInBlack = false;
        HideUI();
    }

    public void FadeInStand()
    {
        ShowUI();
        m_Animator.SetTrigger("FadeInStand");
        _IsInBlack = true;
    }

    public void FadeIn()
    {
        ShowUI();
        //m_Animator.SetTrigger("FadeInStand");
        m_Animator.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        ShowUI();
        m_Animator.SetTrigger("Fade");
    }

    //public override void ShowEvent()
    //{
    //    m_Animator.SetTrigger("Fade");
    //}

    //public override void HideEvent()
    //{
    //    _IsInBlack = false;
    //}
}