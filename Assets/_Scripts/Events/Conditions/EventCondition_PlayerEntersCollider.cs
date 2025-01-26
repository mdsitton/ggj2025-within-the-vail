using UnityEngine;

public class EventCondition_PlayerEntersCollider : EventCondition
{
    protected bool isInside = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInside)
        {
            isInside = true;
            Complete();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isInside)
        {
            isInside = false;
            Release();
        }
    }
}