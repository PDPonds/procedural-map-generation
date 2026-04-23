using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MouseVisual : MonoBehaviour
{
    Transform currentVisual;

    public void SetupPressVisual(Player owner)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.TryGetComponent<PressTrigger>(out var pressTrigger))
            {
                pressTrigger.Setup(owner);
            }
        }
    }

    void Update()
    {
        GameManager.Instance.GetMouseData(out Vector3 hitPoint, out Vector3 hitDirection);
        transform.position = hitPoint;
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
