using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityGame.App.Manager;

public class GameClearUI : IGameUISystem
{
    //[SerializeField]
    //private VideoAdsSettings m_VideoAdsSettings = default;

    [SerializeField]
    private Button m_RestartButton = default;
    [SerializeField]
    private Button m_NextLevelButton = default;
    [SerializeField]
    private Animator m_Animator = default;

    [SerializeField]
    private float m_ShowUIDelay = 0.2f;

    [SerializeField]
    private Image m_BannerAdImage = default;

    //[Header("VideoAds")]
    //[SerializeField]
    //private Text m_VideoAdsTitle = default;
    //[SerializeField]
    //private Button m_VideoAdsButton = default;
    //[SerializeField]
    //private VideoPlayer m_VideoAdsVideoPlayer = default;
    //private int m_VideoIndex = 0;

    private bool m_IsUnlock = false;
    private bool m_IsStageUnlock = false;
    private bool m_IsStartStageUnlock = false;

    public override void GameSetting()
    {
        //m_VideoAdsButton.onClick.AddListener(VideoAdsButtonClick);
        //m_NextLevelButton.gameObject.SetActive(false);

        //#if UNITY_EDITOR
        //        m_BannerAdImage.enabled = true;
        //#else
        //        m_BannerAdImage.enabled = false;
        //#endif
    }

    //public void SetVideoAd()
    //{
    //    m_VideoIndex = SaveManager.GetVideoAdIndex();
    //    int nextAdIndex = m_VideoIndex + 1;
    //    if (nextAdIndex>= m_VideoAdsSettings.m_VideoAdsSettings.Length)
    //    {
    //        nextAdIndex = 0;
    //    }
    //    SaveManager.SetVideoAdIndex(nextAdIndex);
    //    SaveManager.Save();
    //    m_VideoAdsTitle.text = m_VideoAdsSettings.m_VideoAdsSettings[m_VideoIndex].m_AppName;
    //    m_VideoAdsVideoPlayer.clip = m_VideoAdsSettings.m_VideoAdsSettings[m_VideoIndex].m_VideoClip;
    //}

    //public override void ShowEvent()
    //{
    //    if (m_GameManager.GetIsLevelClear())
    //    {
    //        m_LevelCompleteText.text = $"Level {m_GameManager.GetDisplayLevel()} \nComplete!";
    //        m_LevelCompleteText.gameObject.SetActive(true);
    //        m_Animator.SetTrigger("Show");
    //    }
    //    else
    //    {
    //        m_CompleteText.SetActive(true);
    //        Invoke("NextLevelButton", 1f);
    //    }
    //}

    protected override void ShowEvent()
    {
        m_Animator.SetTrigger("Show");
        m_RestartButton.onClick.AddListener(Restart);
        m_NextLevelButton.onClick.AddListener(NextLevelButton);
        //StartCoroutine(ShowUIProcess());
    }

    //IEnumerator ShowUIProcess()
    //{
    //    yield return new WaitForSeconds(m_ShowUIDelay);
    //    m_NextLevelButton.gameObject.SetActive(true);
    //    m_GameManager.CalShowIntersititial();
    //}

    private void Restart()
    {
        GameMediator.m_GameManager.ReplayLevel();
    }

    private void NextLevelButton()
    {
        GameMediator.m_GameManager.NextLevel();
    }

    private void VideoAdsButtonClick()
    {
#if !UNITY_EDITOR
        //Application.OpenURL("https://track.tenjin.io/v0/click/" + m_VideoAdsSettings.m_VideoAdsSettings[m_VideoIndex].m_TenjinTraceCode);
#endif

        //m_TenjinTraceCode
        //#if UNITY_IOS && !UNITY_EDITOR
        //        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + m_VideoAdsSettings.m_VideoAdsSettings[m_VideoIndex].m_AppId);
        //#elif UNITY_ANDROID && !UNITY_EDITOR
        //        Application.OpenURL("market://details?id=" + m_VideoAdsSettings.m_VideoAdsSettings[m_VideoIndex].m_PackageName);
        //#endif
    }
}