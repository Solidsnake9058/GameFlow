namespace UnityGame.App
{
    public enum GameState
    {
        Main,
        Play,
        GameOver,
        GameOverAnimation,
        GameClearAnimation,
        GameClear,
        FadeOut
    }

    public enum GameMode
    {
        Undefined = -1,
        Default,
        Bonus,
    }

    public enum CharaState
    {
        Stand,
        Run,
        Dead,
        Hide
    }

    public class Define
    {
        public static class Path
        {
        }
    }
}