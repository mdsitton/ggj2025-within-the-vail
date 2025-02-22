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

    TargetType TargetType => TargetType.Generic;

    /// <summary>
    /// Gets the current position of the creature.
    /// </summary>
    Vector3 GetPosition();

    /// <summary>
    /// Called by the attacker, through colliders.
    /// </summary>
    /// <param name="damage">Amount of HP dealt by attack</param>
    /// <param name="attacker">Optional target of what is hitting</param>
    void Hit(int damage, ITarget attacker = null);

    UnityEvent HitEvent { get; }
    
    UnityEvent KillEvent { get; }
    UnityEvent ReviveEvent { get; }

}