using UnityGame.App;
using UnityGame.App.Manager;
using UnityGame.Data;
/// <summary>
/// GameEvents are used throughout the game for general game events (game started, game ended, life lost, etc.)
/// </summary>
public struct GameEvent
{
    public TaskEventType EventType;
    public int Id;
    public string EventName;
    public int Value;
    public WindowType WindowName;
    public GameEvent(TaskEventType eventType, int id, int value)
    {
        EventType = eventType;
        Id = id;
        EventName = TaskDetail.GetTaskEventName(EventType, Id);
        Value = value;
        WindowName = WindowType.None;
    }
    static GameEvent e;
    public static void Trigger(TaskEventType eventType, int id, int value)
    {
        e.EventType = eventType;
        e.Id = id;
        e.EventName = TaskDetail.GetTaskEventName(e.EventType, e.Id);
        e.Value = value;
        EventManager.TriggerEvent(e);
    }

    public static void Trigger(TaskEventType eventType, int id, int value, WindowType windowType)
    {
        e.EventType = eventType;
        e.Id = id;
        e.EventName = TaskDetail.GetTaskEventName(e.EventType, e.Id);
        e.Value = value;
        e.WindowName = windowType;
        EventManager.TriggerEvent(e);
    }
}