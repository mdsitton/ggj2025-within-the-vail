using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Override if its possible for a creature to be dead without destroying the script on it.
    /// </summary>
    public virtual bool IsAlive { get => true; }

    /// <summary>
    /// Gets the current position of the creature.
    /// </summary>
    public Vector3 GetPosition();

    /// <summary>
    /// Called by the attacker, through colliders.
    /// </summary>
    /// <param name="damage">Ammount of damager dealt by attack</param>
    public void Hit(int damage);
}