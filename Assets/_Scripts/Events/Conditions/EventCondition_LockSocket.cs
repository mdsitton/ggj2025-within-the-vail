public class EventCondition_LockSocket : EventCondition
{
    public void OnSocketFilled()
    {
        Complete();
    }

    public void OnSocketEmptied()
    {
        Release();
    }
}