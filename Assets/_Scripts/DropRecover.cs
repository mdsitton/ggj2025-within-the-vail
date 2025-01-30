using UnityEngine;

public class DropRecover : MonoBehaviour
{
    public GameObject player;
    public float thresholdY = -10f; // The Y position threshold
    public float distanceInFrontOfPlayer = 0.5f; // Distance to place the object in front of the player
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            if (PlayerManager.instance == null)
            {
                Debug.LogWarning("DropRecover: No player set or found.\nDestroying script.");
                Destroy(this);
            }
            else
                player = PlayerManager.instance.gameObject;
        }
    }

    void Update()
    {
        if (transform.position.y < thresholdY)
            RecoverPosition();
    }

    void RecoverPosition()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = player.transform.position + player.transform.forward * distanceInFrontOfPlayer;
    }
}