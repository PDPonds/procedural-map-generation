using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Transform hotkeyParent;

    public void UdateHotkeyUI(int currentSelectSlot)
    {
        int slotIndex = currentSelectSlot - 1;

        for (int i = 0; i < hotkeyParent.childCount; i++)
        {
            Transform child = hotkeyParent.GetChild(i);
            Transform selectBorder = child.GetChild(1);
            selectBorder.gameObject.SetActive(false);
        }

        Transform currentSlot = hotkeyParent.GetChild(slotIndex);
        Transform currentSlotSelectBorder = currentSlot.GetChild(1);
        currentSlotSelectBorder.gameObject.SetActive(true);
    }
}
