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

    public enum TaskStatus
    {
        NotShow,
        Process,
        Complete,
        Rewarded
    }

    public enum TaskEventType
    {
        TouchClick,
    }

    public enum TaskShowType
    {
        WhenStart,
        WhenClear
    }

    public enum HapticTypes
    {
        Selection, Success, Warning, Failure, LightImpact, MediumImpact, HeavyImpact, RigidImpact, SoftImpact, None
    }

    public enum WindowType
    {
        None,
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