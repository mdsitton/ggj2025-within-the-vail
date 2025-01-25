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

            Quaternion newRot = Quaternion.Euler(0, headTransform.rotation.eulerAngles.y, 0);

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