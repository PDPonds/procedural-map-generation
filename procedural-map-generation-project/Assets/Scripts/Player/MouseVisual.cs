using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MouseVisual : MonoBehaviour
{
    Transform currentVisual;

    public void SetupPressVisual()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.TryGetComponent<PressTrigger>(out var pressTrigger))
            {
                pressTrigger.Setup();
            }
        }
    }

    void Update()
    {
        GameManager.Instance.GetMouseData(out Vector3 hitPoint, out Vector3 dirToMouse);
        transform.position = hitPoint;
        LookAt(dirToMouse);
    }

    public void LookAt(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void SwitchMouseVisual(PlayerSelectSlot slot)
    {
        int visualID = (int)slot;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }

        Transform visual = transform.GetChild(visualID);
        currentVisual = visual;
        visual.gameObject.SetActive(true);
    }

    public bool CanPressObject()
    {
        if (currentVisual != null)
        {
            currentVisual.TryGetComponent<PressTrigger>(out PressTrigger pressTrigger);
            return pressTrigger.NoColliding();
        }
        return false;
    }

}
