public class EventCondition_Undo : EventCondition
{
    public EventContainer containerToWatch;

    public override void Initialize(EventContainer container)
    {
        containerToWatch.onRelease.AddListener(Complete);
        containerToWatch.onComplete.AddListener(Release);
    }
}