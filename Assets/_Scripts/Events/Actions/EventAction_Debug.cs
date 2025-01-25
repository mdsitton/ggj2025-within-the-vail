using UnityEngine;

public class EventAction_Debug : EventAction
{
    public string message;
    public DebugMessageType debugMessageType;

    public override void Execute()
    {
        switch (debugMessageType)
        {
            case DebugMessageType.Log:
                Debug.Log(message);
                break;
            case DebugMessageType.Warning:
                Debug.LogWarning(message);
                break;
            case DebugMessageType.Error:
                Debug.LogError(message);
                break;
        }
    }

    public enum DebugMessageType
    {
        Log,
        Warning,
        Error
    }
}