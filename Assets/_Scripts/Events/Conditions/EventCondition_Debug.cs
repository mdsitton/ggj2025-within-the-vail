public class EventCondition_Debug : EventCondition
{
    public void DebugComplete()
    {
        Complete();
    }

    public void DebugRelease()
    {
        Release();
    }
}