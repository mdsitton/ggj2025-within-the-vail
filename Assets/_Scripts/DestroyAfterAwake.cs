using UnityEngine;

public class DestroyAfterAwake : MonoBehaviour
{
    public float delay = 0.1f;

    private void Awake()
    {
        Destroy(gameObject, delay);
    }
}
