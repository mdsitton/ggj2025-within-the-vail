using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;

public class RotatingStatue : MonoBehaviour
{
    public XRKnob knob;
    [Tooltip("Plays when locked in position")]
    public AudioSource lockInPlaceSFX;

    [Range(0, 1)]
    public float targetRotation;

    [Range(0.01f,0.2f)]
    public float leniance;

    public bool lockInPlace = true;

    public UnityEvent inPlace;
    public UnityEvent outOfPlace;

    private bool isInPosition = false;

    public float StatueAngle { get => knob.value * 1; }

    public void Start()
    {
        OnStatueMoved();
    }
    
    public void OnStatueMoved ()
    {
        Debug.Log((Mathf.Abs(StatueAngle - targetRotation) <= leniance ? "Pass" : "Fail") + "\nAngle: " + Mathf.Abs(StatueAngle - targetRotation));

        if (Mathf.Abs(StatueAngle - targetRotation) <= leniance)
        {
            if (!isInPosition)
            {
                if (lockInPlace)
                {
                    knob.value = targetRotation;
                    //knob.enabled = false;
                    if (lockInPlaceSFX != null)
                        lockInPlaceSFX.Play();
                }
                isInPosition = true;
                inPlace.Invoke();
            }
        }
        else
        {
            if (isInPosition)
            {
                isInPosition = false;
                outOfPlace.Invoke();
            }
        }
    }

    private float MinAngularDistance(float a, float b)
    {
        return Mathf.Abs((a - b + 180) % 360 - 180); // taken from online
    }
}