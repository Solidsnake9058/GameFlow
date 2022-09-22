using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingUI : IGameUISystem
{
    [SerializeField]
    private Animator m_Animator = default;

    protected override void ShowEvent()
    {
        m_Animator.SetTrigger("Show");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
