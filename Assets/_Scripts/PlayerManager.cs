using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour, ITarget
{
    public static PlayerManager instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    public int health = 100;
    
    // ITarget implementation
    Vector3 ITarget.GetPosition() => transform.position;
    
    TargetType ITarget.TargetType => TargetType.Player;
    
    bool ITarget.IsAlive => health > 0;

    public void Hit(int damage, ITarget attacker = null)
    {
        health -= damage;
        if (health <= 0)
        {
            onKill.Invoke();
            Destroy(gameObject);
        }
        else
        {
            onHit.Invoke();
        }
    }

    public UnityEvent onHit;
    public UnityEvent onKill;
    public UnityEvent onRevive;

    UnityEvent ITarget.HitEvent => onHit;
    UnityEvent ITarget.KillEvent => onKill;
    UnityEvent ITarget.ReviveEvent => onRevive;
}