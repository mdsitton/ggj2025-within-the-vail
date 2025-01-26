public class EventCondition_LockSocket : EventCondition
{
    public void OnSocketFilled()
    {
        UnityEngine.Debug.Log("Filled " + name);
        Complete();
    }

    public void OnSocketEmptied()
    {
        UnityEngine.Debug.Log("Removed " + name);
        Release();
    }
}