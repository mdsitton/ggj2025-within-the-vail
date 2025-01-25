using UnityEngine;

public class EventCondition_targetKilled : EventCondition
{
    public GameObject[] gameObjectsWithITargetScript;
    protected ITarget[] targets;

    public override void Initialize(EventContainer container)
    {
        if (gameObjectsWithITargetScript.Length < 1)
        {
            Debug.LogWarning("No GameObjects are set in array!", this);
            Destroy(this);
        }

        targets = new ITarget[gameObjectsWithITargetScript.Length];
        for (int i=0; i<gameObjectsWithITargetScript.Length; i++)
        {
            ITarget target = gameObjectsWithITargetScript[i].GetComponent<ITarget>();
            if (target != null)
            {
                target.KillEvent.AddListener(Check);
                target.ReviveEvent.AddListener(Check);
                targets[i] = target;
            }
            else
                Debug.LogWarning($"GameObject {gameObjectsWithITargetScript[i].name} does not contain an ITarget!", this);
        }
    }

    protected void Check ()
    {
        foreach (ITarget target in targets)
            if (target != null && target.IsAlive)
            {
                Release();
                return;
            }
        Complete();
    }
}