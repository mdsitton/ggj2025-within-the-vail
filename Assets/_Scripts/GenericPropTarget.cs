using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GenericPropTarget : MonoBehaviour, ITarget
{
    public int hp = 10;
    [Tooltip("The minimum velocity required to cause collision damage.")]
    public float minVelocity = 2;
    [Tooltip("The amount of damage per m/s of collision velocity.")]
    public float collisionDamageModifier = 2;
    [Tooltip("The duration of invulnerability after being grabbed. Prevents shattering on the surface it's resting on.")]
    public float grabInvulnerabilityDuration = 1;
    [Tooltip("The sound effect to play when the object is destroyed.")]
    public AudioClip deathSFX;

    private Coroutine endInvincibility;
    private bool ignoreCollisionDamage = false;
    private int currHP;

    #region Initialization
    void Start ()
    {
        currHP = hp;

        if (TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            grabInteractable.selectEntered.AddListener(StartGrab);
            grabInteractable.selectExited.AddListener(StopGrab);
        }
    }
    #endregion

    #region Input Methods
    public void StartGrab(SelectEnterEventArgs grabData)
    {
        if (grabInvulnerabilityDuration > 0)
        {
            if (endInvincibility != null)
                StopCoroutine(endInvincibility);
            else
                ignoreCollisionDamage = true;
            endInvincibility = StartCoroutine(EndInvincibility());
        }
    }

    public void StopGrab(SelectExitEventArgs grabData)
    {
        StopGrab();
    }

    public void StopGrab()
    {
        StopCoroutine(endInvincibility);
        ignoreCollisionDamage = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ignoreCollisionDamage)
            return;
        if (collision.relativeVelocity.magnitude > minVelocity)
            Hit(Mathf.FloorToInt(collision.relativeVelocity.magnitude * collisionDamageModifier));
    }
    #endregion

    #region Methods
    private IEnumerator EndInvincibility ()
    {
        yield return new WaitForSeconds(grabInvulnerabilityDuration);
        ignoreCollisionDamage = false;
    }

    private void OnKill()
    {
        onKill.Invoke();
        AudioSource.PlayClipAtPoint(deathSFX, transform.position);
        Destroy(gameObject);
    }
    #endregion

    #region ITarget Implementation
    public UnityEvent onHit;
    public UnityEvent onKill;
    public UnityEvent onRevive;
    UnityEvent ITarget.HitEvent => onHit;
    UnityEvent ITarget.KillEvent => onKill;
    UnityEvent ITarget.ReviveEvent => onRevive;

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Hit(int damage, ITarget attacker = null)
    {
        currHP -= damage;
        if (currHP > 0)
            onHit.Invoke();
        else
            OnKill();
    }
    #endregion
}