using UnityEngine;

public class EventCondition_DamageableKilled : EventCondition
{
    public GameObject[] gameObjectsWithIDamageableScript;
    protected IDamageable[] damageables;

    public override void Initialize(EventContainer container)
    {
        if (gameObjectsWithIDamageableScript.Length < 1)
        {
            Debug.LogWarning("No GameObjects are set in array!", this);
            Destroy(this);
        }

        damageables = new IDamageable[gameObjectsWithIDamageableScript.Length];
        for (int i=0; i<gameObjectsWithIDamageableScript.Length; i++)
        {
            IDamageable damageable = gameObjectsWithIDamageableScript[i].GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.KillEvent.AddListener(Check);
                damageable.ReviveEvent.AddListener(Check);
                damageables[i] = damageable;
            }
            else
                Debug.LogWarning($"GameObject {gameObjectsWithIDamageableScript[i].name} does not contain an IDamageable!", this);
        }
    }

    protected void Check ()
    {
        foreach (IDamageable damageable in damageables)
            if (damageable != null && damageable.IsAlive)
            {
                Release();
                return;
            }
        Complete();
    }
}