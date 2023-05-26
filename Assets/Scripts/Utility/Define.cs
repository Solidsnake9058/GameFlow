namespace UnityGame.App
{
    public enum GameState
    {
        Start,
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
        RunCheck,
        Dead,
        Hide
    }

    public enum CreatorMode
    {
        Editing,
        TestPlaying
    }

    public class Define
    {
        public static class Path
        {
        }
    }
}