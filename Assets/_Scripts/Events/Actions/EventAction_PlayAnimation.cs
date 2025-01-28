using UnityEngine;

public class EventAction_PlayAnimation : EventAction
{
    public Animation[] animations;

    public override void Execute()
    {
        foreach (Animation animation in animations)
            animation.Play();
    }
}