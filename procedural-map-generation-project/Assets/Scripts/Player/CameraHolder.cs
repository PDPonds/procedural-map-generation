using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    Transform target;
    [SerializeField] float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        FollowTarget();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void FollowTarget()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPos;
        }
    }
}
