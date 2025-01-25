public class EventCondition_Or : EventCondition
{
    public EventCondition[] conditions;

    public override void Initialize(EventContainer container)
    {
        foreach (EventCondition condition in conditions)
        {
            condition.onComplete.AddListener(Check);
            condition.onRelease.AddListener(Check);
        }
    }

    public void Check()
    {
        foreach (EventCondition condition in conditions)
            if (condition.IsCompleted())
                Complete(); ;

        Release();
    }


}