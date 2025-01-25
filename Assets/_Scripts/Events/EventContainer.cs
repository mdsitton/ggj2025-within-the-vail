using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Nomenclature guide:
/// <br>Conditions are CHECKED, which results in them being COMPLETED on a pass, or RELEASED on a fail.</br>
/// <br>Actions are EXECUTED. No undos. But may be executed multiple times depending on settings.</br>
/// </summary>
public class EventContainer : MonoBehaviour
{
    public List <EventCondition> conditions = new List <EventCondition> ();
    public List <EventAction> actions = new List <EventAction> ();

    public bool executeOnlyOnce = true;

    private bool isCompleted = false;
    private bool hasExecuted = false;

    public UnityEvent onComplete;
    public UnityEvent onRelease;

    public void Start()
    {
        foreach (EventCondition condition in conditions)
        {
            condition.onComplete.AddListener(CheckConditions);
            condition.onRelease.AddListener(CheckConditions);

            condition.Initialize(this);
        }
    }

    public void CheckConditions ()
    {
        foreach (EventCondition condition in conditions)
            if (!condition.IsCompleted())
            {
                ReleaseActions ();
                return;
            }

        ExecuteActions();
    }

    public bool IsCompleted ()
    {
        return isCompleted || (hasExecuted && executeOnlyOnce);
    }

    public void ExecuteActions ()
    {
        if ((hasExecuted && executeOnlyOnce) || isCompleted)
            return;

        isCompleted = true;
        hasExecuted = true;
        foreach(EventAction action in actions)
            action.Execute ();

        onComplete.Invoke();
    }

    public void ReleaseActions ()
    {
        if (!isCompleted || (isCompleted && executeOnlyOnce))
            return;
        isCompleted = false;
        onRelease.Invoke ();
    }
}

[System.Serializable]
public abstract class EventCondition : MonoBehaviour
{
    [Header("EventCondition")]

    public bool latchComplete = true;
    [Tooltip("Only applies when latchComplete is set to false.")]
    public bool canRecomplete = true;

    protected bool isCompleted = false;
    private bool hasCompleted = false;

    public UnityEvent onComplete;
    public UnityEvent onRelease;

    public virtual void Initialize (EventContainer container) {}

    public virtual bool IsCompleted()
    {
        if ((hasCompleted && latchComplete) || isCompleted)
            return true;
        else
            return false;
    }

    protected void Complete ()
    {
        if (isCompleted)
            return;
        isCompleted = true;
        hasCompleted = true;
        onComplete.Invoke ();
    }

    protected void Release ()
    {
        if (latchComplete || !isCompleted)
            return;
        isCompleted = false;
        onRelease.Invoke();
    }
}

[System.Serializable]
public abstract class EventAction : MonoBehaviour
{
    public abstract void Execute();
}