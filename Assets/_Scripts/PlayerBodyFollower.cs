using UnityEngine;

public class PlayerBodyFollower : MonoBehaviour
{
    public Transform headTransform;
    public Transform handTransform;

    public FollowMode followMode;

    public Vector3 offset;
    [Tooltip("If enabled, will match the height of a percentage of the camera, instead of using the fixed offset.y value")]
    public bool useHeightPercent = true;
    public float heightPercent = 0.5f;
    [Tooltip("Maximum angle the followed object is allowed to be from the camera angle.")]
    [Range(0,40)]
    public float angleMax = 20; // Adds a 'memory' effect. Keeps it still during short motions.

    void Start()
    {
        if (headTransform == null)
            Destroy(this);
    }

    void Update()
    {
        if (followMode == FollowMode.Body)
        {
            Vector3 newPos = headTransform.position + (headTransform.right * offset.x) + (headTransform.forward * offset.z);
            //newPos.y = (headTransform.parent.position.y + offset.y);
            newPos.y = headTransform.parent.position.y + (headTransform.localPosition.y * heightPercent);

            Quaternion newRot = Quaternion.Euler(0, headTransform.rotation.eulerAngles.y, 0); // Flatters the cameras rotation
            if (Mathf.Abs(newRot.eulerAngles.y - transform.rotation.eulerAngles.y) > angleMax) // If within angle limit, don't change followers rotation
                newRot = transform.rotation;

            // Animate
            newRot = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime*10);

            transform.SetPositionAndRotation(newPos, newRot);
        }
        else
        {
            Debug.LogWarning($"FollowMode {followMode} not supported.");
            followMode = FollowMode.Body;
        }
    }
}

public enum FollowMode
{
    Body,
    Head,
    Arm,
    Hand
}