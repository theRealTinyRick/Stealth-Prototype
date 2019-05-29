using UnityEngine;

public enum DebugType
{
    Log, 
    Warning,
    Error
}

public class DebugLogComponent : MonoBehaviour
{
    public DebugType debugType;

    public void DebugLog(string message)
    {
        switch(debugType)
        {
            case DebugType.Log:
                Debug.Log(message);
                break;
            case DebugType.Warning:
                Debug.LogWarning(message);
                break;
            case DebugType.Error:
                Debug.LogError(message);
                break;
        }
    }
}
