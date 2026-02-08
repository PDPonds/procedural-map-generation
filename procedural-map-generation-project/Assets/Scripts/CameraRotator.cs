using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float rotateSpeed;
    public float zoomSpeed;
    public float minFOV;
    public float maxFOV;
    float targetFOV = 0;
    bool canRotate;
    public bool isAutoRotate;
    public bool flipRotation;

    private void Awake()
    {
        targetFOV = Camera.main.fieldOfView;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFOV -= 5;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFOV += 5;
        }
        targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        Camera.main.fieldOfView =
        Mathf.Lerp(Camera.main.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        if (!isAutoRotate)
        {
            if (Input.GetMouseButtonDown(0))
            {
                canRotate = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                canRotate = false;
            }

            if (canRotate)
            {
                float yInput = Input.GetAxis("Mouse X");
                if (flipRotation) yInput = yInput * -1;
                transform.Rotate(0, yInput * rotateSpeed * Time.deltaTime, 0);
            }
        }
        else
        {
            float yInput = 1;
            if (flipRotation) yInput = yInput * -1;
            transform.Rotate(0, yInput * rotateSpeed * Time.deltaTime, 0);
        }

        
    }
}
