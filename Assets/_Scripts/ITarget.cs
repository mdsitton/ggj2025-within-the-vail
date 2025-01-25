using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A target is an object that can be attacked or healed.
/// </summary>
public interface ITarget
{
    /// <summary>
    /// Explicitly implement if it's possible for a creature to be dead without destroying the script on it.
    /// </summary>
    bool IsAlive => true;

    /// <summary>
    /// Gets the current position of the creature.
    /// </summary>
    Vector3 GetPosition();

    /// <summary>
    /// Called by the attacker, through colliders.
    /// </summary>
    /// <param name="damage">Amount of HP dealt by attack</param>
    void Hit(int damage);

    UnityEvent HitEvent { get; }
    
    UnityEvent KillEvent { get; }
    UnityEvent ReviveEvent { get; }

}