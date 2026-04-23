using UnityEngine;

public class InputManager : MonoBehaviour
{
    InputSystem_Actions action;

    private void OnEnable()
    {
        if (action == null)
        {
            GameManager gm = GetComponent<GameManager>();
            action = new InputSystem_Actions();

            action.Player.Move.performed += i => gm.moveInput = i.ReadValue<Vector2>();

            action.Player.Slot1.performed += i => gm.SelectHotkeySlot(1);
            action.Player.Slot2.performed += i => gm.SelectHotkeySlot(2);
            action.Player.Slot3.performed += i => gm.SelectHotkeySlot(3);
            action.Player.Slot4.performed += i => gm.SelectHotkeySlot(4);
            action.Player.Slot5.performed += i => gm.SelectHotkeySlot(5);
            action.Player.Slot6.performed += i => gm.SelectHotkeySlot(6);
            action.Player.Slot7.performed += i => gm.SelectHotkeySlot(7);
            action.Player.MouseScollingUp.performed += i =>
            {
                float increasCount = i.ReadValue<Vector2>().y;
                int count = Mathf.FloorToInt(increasCount);
                gm.IncreaseHotKeySlot(count);
            };
            action.Player.UseSlot.performed += i => gm.UseSlot();
            action.Player.MousePos.performed += i => gm.mousePosInput = i.ReadValue<Vector2>();
        }
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }
}
