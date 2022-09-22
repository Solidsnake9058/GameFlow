using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitManager : MonoBehaviour {

    [SerializeField]
    private bool m_IsShowMainUIWhenStart = false;

    private void Awake()
    {
#if Debug
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    void Start()
    {
        Init();
        Invoke("OnInitEnd", 0.2f);
    }

    void Update()
    {
        //if (FirebaseInit._Instance.IsInitializationed())
        //{
            //OnInitEnd();
        //}
    }

    private void Init()
    {
        //PlayerPrefs.SetInt("StageIndex", SaveManager.CurrentStageNo);
        PlayerPrefs.SetInt("MainUI", m_IsShowMainUIWhenStart ? 1 : 0);

#if !NO_ADS
        //IronSourceManager.Init();
        //IronSourceManager.LoadBanner(IronSourceBannerPosition.BOTTOM);
#endif
    }

    private void OnInitEnd()
    {
        SceneManager.LoadScene("Game");
    }
}
