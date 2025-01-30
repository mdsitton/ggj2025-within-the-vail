public class EventCondition_RotatingStatue : EventCondition
{
    public RotatingStatue statue;

    public override void Initialize()
    {
        statue.inPlace.AddListener(Complete);
        statue.outOfPlace.AddListener(Release);
    }
}