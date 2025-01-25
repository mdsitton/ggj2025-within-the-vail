using UnityEngine;

public class Candle : MonoBehaviour, IDamageable
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
    #endregion

    #region Public Methods
    public void Light ()
    {
        isLit = true;
        flame.SetActive(true);
    }

    public void Extinguish ()
    {
        isLit = false;
        flame.SetActive(false);
    }
    #endregion

    #region IDamageable Implementation
    public bool IsAlive { get => isLit; }

    public Vector3 GetPosition() { return transform.position; }

    public void Hit (int damage)
    {
        Debug.Log("Candle was hit!");
        if (damage > 1)
            Extinguish();
    }
    #endregion
}