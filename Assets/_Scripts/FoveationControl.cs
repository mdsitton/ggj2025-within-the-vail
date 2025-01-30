using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FoveationControl : MonoBehaviour
{
    public XRDisplaySubsystem xrDisplaySubsystem;
    public float strength = 1.0f;
    public bool tryAgain = false;

    void Start()
    {
        Debug.Log(SystemInfo.foveatedRenderingCaps);

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart ()
    {
        yield return new WaitForSeconds(1.0f);
        FindSubsystem();
        SetFRLevel();
    }

    public void FindSubsystem ()
    {
        // Find the XR display subsystem
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems<XRDisplaySubsystem>(xrDisplaySubsystems);

        if (xrDisplaySubsystems.Count < 1)
        {
            Debug.LogError("No XR display subsystems found.");
            return;
        }
        foreach (var subsystem in xrDisplaySubsystems)
        {
            if (subsystem.running)
            {
                Debug.Log("Subsystem found!");
                xrDisplaySubsystem = subsystem;
                break;
            }
        }
    }

    public void SetFRLevel()
    {
        if (xrDisplaySubsystem != null)
        {
            Debug.Log("Current Foveated Rendering Level: " + xrDisplaySubsystem.foveatedRenderingLevel);
            xrDisplaySubsystem.foveatedRenderingLevel = strength;
            Debug.Log("Set Foveated Rendering Level to " + strength);
        }
    }
}