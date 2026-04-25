using Unity.AI.Navigation;
using UnityEngine;

public enum Faction
{
    None, Player, Enemy
}

public class GameManager : Singleton<GameManager>
{

    public GameData gameData;

    [HideInInspector] public Player player;
    [HideInInspector] public Vector2 mousePosInput;
    [HideInInspector] public Vector2 moveInput;

    [HideInInspector] public Ship ship;

    int currentSelectSlot = 1;

    private void Awake()
    {
        SpawningPlayer(new Vector3(32, 0, 32), new Vector3(32, 0, 32), Vector3.zero);
    }

    void SpawningPlayer(Vector3 playerPos, Vector3 shipPos, Vector3 shipDir)
    {
        GameObject playerObj = Instantiate(gameData.player_Prefab.gameObject, new Vector3(32, 0, 32), Quaternion.identity);
        GameObject camHolderObj = Instantiate(gameData.cameraHolder_Prefab.gameObject, Vector3.zero, Quaternion.identity);
        GameObject mouseVisualObj = Instantiate(gameData.mouseVisual_Prefab.gameObject, Vector3.zero, Quaternion.identity);
        GameObject shipObj = Instantiate(gameData.ship_Prefab.gameObject, shipPos, Quaternion.Euler(shipDir));
        ship = shipObj.GetComponent<Ship>();
        Player player = playerObj.GetComponent<Player>();
        this.player = player;
        MouseVisual mouseVisual = mouseVisualObj.GetComponent<MouseVisual>();
        player.SetupOnSpawning(mouseVisual);
        mouseVisual.SetupPressVisual();
        CameraHolder cameraHolder = camHolderObj.GetComponent<CameraHolder>();
        cameraHolder.SetTarget(playerObj.transform);
        SelectHotkeySlot(1);
    }

    public void SelectHotkeySlot(int slotID)
    {
        if (player != null)
        {
            PlayerUI playerUI = player.GetComponent<PlayerUI>();
            currentSelectSlot = slotID;
            playerUI.UdateHotkeyUI(currentSelectSlot);
            PlayerSelectSlot slot = (PlayerSelectSlot)(slotID - 1);
            player.SwitchSlot(slot);
        }
    }

    public void IncreaseHotKeySlot(int increaseCount)
    {
        if (player != null)
        {
            int temp = currentSelectSlot;
            temp += increaseCount;
            if (temp < 1) temp = 1;
            if (temp > 7) temp = 7;
            SelectHotkeySlot(temp);
        }
    }

    public void GetMouseData(out Vector3 hitPoint, out Vector3 dirToMouse)
    {
        dirToMouse = Vector3.zero;
        hitPoint = Vector3.zero;
        if (player != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosInput);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, gameData.mouseLayerMark))
            {
                Vector3 hitpoint = hit.point;
                Vector3 dir = hitpoint - player.transform.position;
                hitPoint = hitpoint;
                dirToMouse = dir.normalized;
            }
        }
    }

    GameObject GetPressObject(PlayerSelectSlot slot)
    {
        if (slot == PlayerSelectSlot.Fence) { return gameData.fencePrefab.gameObject; }
        else if (slot == PlayerSelectSlot.Cannon) { return gameData.cannonPrefab.gameObject; }
        else if (slot == PlayerSelectSlot.Barrel) { return gameData.barrelPrefab.gameObject; }
        else if (slot == PlayerSelectSlot.Thorn) { return gameData.thornPrefab.gameObject; }
        else if (slot == PlayerSelectSlot.Flag) { return gameData.flagPrefab.gameObject; }

        return null;
    }

    public void UseSlot()
    {
        if (player != null)
        {
            if (player.IsSlot(PlayerSelectSlot.Melee))
            {
                if (player.CanMeleeAttack())
                {
                    StartCoroutine(player.MeleeAttack());
                }
            }
            else if (player.IsSlot(PlayerSelectSlot.Gun))
            {
                if (player.CanRangeAttack())
                {
                    player.RangeAttack();
                }
            }
            else if (player.IsSlot(PlayerSelectSlot.Cannon) ||
                player.IsSlot(PlayerSelectSlot.Fence) ||
                player.IsSlot(PlayerSelectSlot.Thorn) ||
                player.IsSlot(PlayerSelectSlot.Barrel) ||
                player.IsSlot(PlayerSelectSlot.Flag))
            {
                if (player.CanPressObj())
                {
                    PlayerSelectSlot slot = player.GetPlayerSelectSlot();
                    GameObject prefab = GetPressObject(slot);
                    GetMouseData(out Vector3 hitPoint, out Vector3 dirToMouse);
                    GameObject obj = Instantiate(prefab, hitPoint, Quaternion.Euler(dirToMouse));

                }
            }
        }
    }

}
