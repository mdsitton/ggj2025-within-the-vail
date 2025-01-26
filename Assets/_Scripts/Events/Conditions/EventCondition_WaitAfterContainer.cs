using System.Collections;
using UnityEngine;

public class EventCondition_WaitAfterContainer : EventCondition
{
    [Header("Wait After Container")]
    public EventContainer containerToWatch;

    public float delay;

    [Tooltip("If this container executes while waiting, will block this condition from completing.")]
    public EventContainer containerBlocker;

    [Tooltip("If true, the blocking container will prevent ")]
    public bool blockLatch = true;

    private bool isBlocked = false;

    public override void Initialize(EventContainer container)
    {
        containerToWatch.onComplete.AddListener(StartDelayCoroutine);
        containerBlocker.onComplete.AddListener(BlockCompletion);
        containerBlocker.onRelease.AddListener(UnblockCompletion);
    }

    private void StartDelayCoroutine()
    {
        StartCoroutine(DelayComplete());
    }

    private void BlockCompletion()
    {
        isBlocked = true;
    }

    private void UnblockCompletion ()
    {
        if (!blockLatch)
            isBlocked = false;
    }

    private IEnumerator DelayComplete ()
    {
        yield return new WaitForSeconds(delay);

        if (!isBlocked)
            Complete();
    }
}