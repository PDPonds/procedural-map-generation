using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    Transform target;
    [SerializeField] float smoothSpeed = 0.125f;

    private void Update()
    {
        if (GameManager.Instance.canRatateCam)
        {
            float roationX = GameManager.Instance.mouseDelta.normalized.x;
            transform.Rotate(0, roationX * 100 * Time.deltaTime, 0);
        }
    }

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
