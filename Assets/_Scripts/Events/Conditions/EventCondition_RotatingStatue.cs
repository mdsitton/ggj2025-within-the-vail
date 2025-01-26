public class EventCondition_RotatingStatue : EventCondition
{
    public RotatingStatue statue;

    public override void Initialize(EventContainer container)
    {
        statue.inPlace.AddListener(Complete);
        statue.outOfPlace.AddListener(Release);
    }
}