using UnityEngine;
using UnityEngine.Events;

public class Candle : MonoBehaviour, ITarget
{
    #region Properties
    public GameObject flame;
    public Light lightSource;

    [Tooltip("Higher -> More flicker. Flicker range is [1-%, 1]")]
    [Range(0f, 1f)]
    public float flickerPercent = .5f;
    [Range(0.1f,5f)]
    public float flickerSpeed = 1;

    public bool isLit = true;

    private float intensity;
    private float seed;
    #endregion

    #region Unity Methods
    void Start()
    {
        seed = Random.Range(0, 10000);
        intensity = lightSource.intensity;

        if (isLit)
            Light();
        else
            Extinguish();
    }

    void Update()
    {
        if (isLit)
        {
            lightSource.intensity = (intensity * (1-flickerPercent)) + (Mathf.PerlinNoise1D(seed + (Time.time * flickerSpeed)) * flickerPercent);
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.isTrigger && other.TryGetComponent(out Candle otherCandle))
        {
            if (isLit && !otherCandle.isLit)
            {
                otherCandle.Light();
            }
        }
    }
    #endregion

    #region Public Methods
    public void Light ()
    {
        isLit = true;
        flame.SetActive(true);
        onRevive.Invoke();
    }

    public void Extinguish ()
    {
        isLit = false;
        flame.SetActive(false);
        onKill.Invoke();
    }
    #endregion

    #region ITarget Implementation
    public UnityEvent onHit;
    public UnityEvent onKill;
    public UnityEvent onRevive;

    public bool IsAlive { get => isLit; }

    public Vector3 GetPosition() { return transform.position; }

    public void Hit (int damage, ITarget attacker = null)   
    {
        Debug.Log(attacker);
        onHit.Invoke();
        if (damage > 1)
            Extinguish();
    }

    UnityEvent ITarget.HitEvent => onHit;
    UnityEvent ITarget.KillEvent => onKill;
    UnityEvent ITarget.ReviveEvent => onRevive;

#endregion
}