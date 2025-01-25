using UnityEngine;

public class EventAction_SetActive : EventAction
{
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    public override void Execute()
    {
        foreach (GameObject go in objectsToActivate)
            go.SetActive(true);
        foreach (GameObject go in objectsToDeactivate)
            go.SetActive(false);
    }
}