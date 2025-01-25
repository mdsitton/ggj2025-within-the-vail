using UnityEngine;

public class PlayerBodyFollower : MonoBehaviour
{
    public Transform headTransform;
    public Transform handTransform;

    public FollowMode followMode;

    public Vector3 offset;

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
            newPos.y = (headTransform.parent.position.y + offset.y);

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