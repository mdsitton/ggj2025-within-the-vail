using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CollisionInteraction : MonoBehaviour
{
    #region Properties
    public AudioClip[] collisionClips;
    public AudioMixerGroup mixerGroup;

    [Range(0,1)]
    public float maxVolume = 1;
    [Range(-0.5f, 0.5f)]
    public float pitchOffset = 0;
    [Range(0, 1)]
    public float pitchVariance= 0.15f;
    [Min(0)]
    public float minAudioVelocity = 0.3f;
    [Min(0)]
    public float maxAudioVelocity = 10f;
    public float grabVelocity = 5;

    protected AudioSource source;
    protected Hand hand = Hand.None;
    protected Rigidbody rb;
    #endregion

    #region Initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1;
            source.minDistance = 0.5f;
            source.maxDistance = 100f;
            if (mixerGroup != null)
                source.outputAudioMixerGroup = mixerGroup;
        }

        if (TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            grabInteractable.selectEntered.AddListener(StartGrab);
            grabInteractable.selectExited.AddListener(StopGrab);
        }
    }
    #endregion

    #region Methods
    public void Collide(float relativeVelocity)
    {
        if (relativeVelocity < minAudioVelocity)
            return;

        if (relativeVelocity > maxAudioVelocity)
            relativeVelocity = maxAudioVelocity;

        source.pitch = 1 + pitchOffset + Random.Range(-pitchVariance, pitchVariance);
        source.volume = maxVolume * (relativeVelocity - minAudioVelocity) / (maxAudioVelocity - minAudioVelocity);
        source.PlayOneShot(collisionClips[Random.Range(0, collisionClips.Length)]);
    }
    #endregion

    #region Input Events
    public void StartGrab (SelectEnterEventArgs grabData)
    {
        hand = (grabData.interactorObject.interactionLayers & InteractionLayerMask.NameToLayer("Right")) == 0 ? Hand.Right : Hand.Left;

        if (grabVelocity > 0)
            Collide(grabVelocity);
    }

    public void StopGrab(SelectExitEventArgs grabData)
    {
        hand = Hand.None;
    }

    public void StopGrab()
    {
        hand = Hand.None;
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"OnCollisionEnter() with {collision.gameObject.name}\nRelative Velocity: {collision.relativeVelocity.magnitude}\nOthers Velocity: {collision.rigidbody?.velocity}");
        if (hand == Hand.None)
            Collide(collision.relativeVelocity.magnitude);
        else
            Collide((GetCurrentVelocity() - (collision.rigidbody != null ? collision.rigidbody.velocity : Vector3.zero)).magnitude);
    }
    #endregion

    #region Helpers
    public Vector3 GetCurrentVelocity()
    {
        Vector3 velocity = Vector3.zero;
        if (hand == Hand.None)
        {
            if (rb != null)
                velocity = rb.velocity;
        }
        else
        {
            InputDevice ControllerDevice = InputDevices.GetDeviceAtXRNode(hand == Hand.Right ? XRNode.RightHand : XRNode.LeftHand);
            if (ControllerDevice != null)
                ControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out velocity);
        }
        return velocity;
    }

    protected enum Hand
    {
        None,
        Left,
        Right
    }
    #endregion
}