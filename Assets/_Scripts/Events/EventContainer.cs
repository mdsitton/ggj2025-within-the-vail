using System.Collections.Generic;
using UnityEngine;

public class EventContainer : MonoBehaviour
{
    public List <EventCondition> conditions = new List <EventCondition> ();
    public List <EventAction> actions = new List <EventAction> ();

    [Tooltip("Time between condition checks. Set to -1 to disable.")]
    public float checkDelay = 0.2f;

    public bool executeOnlyOnce = true;

    private float nextCheck = -1;
    private bool isExecuted = false;
    private bool hasExecuted = false;

    public void OnEnable()
    {
        nextCheck = Time.time + 0.2f;
    }

    public void Update()
    {
        if (checkDelay > 0 && Time.time >= nextCheck)
        {
            if (CheckConditions())
                ExecuteActions();
            else
                isExecuted = false;
        }
    }

    public bool CheckConditions ()
    {
        nextCheck = Time.time + 0.2f;

        foreach (EventCondition condition in conditions)
            if (!condition.IsCompleted())
                return false;

        return true;
    }

    public bool IsCompleted ()
    {
        return isExecuted;
    }

    public void ExecuteActions ()
    {
        if ((hasExecuted && executeOnlyOnce) && !isExecuted)
            return;

        isExecuted = true;
        hasExecuted = true;
        foreach(EventAction action in actions)
            action.Execute ();
    }
}

[System.Serializable]
public abstract class EventCondition : MonoBehaviour
{
    public abstract bool IsCompleted();
}

[System.Serializable]
public abstract class EventAction : MonoBehaviour
{
    public abstract void Execute();
}