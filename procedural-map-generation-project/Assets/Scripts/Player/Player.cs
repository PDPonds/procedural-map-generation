using System.Collections;
using UnityEngine;

public enum PlayerSelectSlot
{
    Melee, Gun, Fence, Cannon, Barrel, Thorn, Flag
}

public class Player : MonoBehaviour
{
    public PlayerData playerData;

    CharacterController controller;
    MouseVisual mouseVisual;

    Vector3 moveDir;
    float velocity;

    PlayerSelectSlot slot;


    [Header("===== Melee Attack =====")]
    float curAttackDelay = 0;
    [SerializeField] GameObject meleeHitbox;
    [Header("===== Range Attack =====")]
    [SerializeField] float curRangeDelay = 0;
    [SerializeField] Transform rangeAttackSpawnPosition;

    public void SetupOnSpawning(MouseVisual mouseVisual)
    {
        controller = GetComponent<CharacterController>();
        this.mouseVisual = mouseVisual;
    }

    private void Update()
    {
        MovementHandle();
        RotationHandle();
        UpdateSlot();
        DecreaseAttackDelay();
    }

    void MovementHandle()
    {
        moveDir = Camera.main.transform.forward * GameManager.Instance.moveInput.y;
        moveDir = moveDir + Camera.main.transform.right * GameManager.Instance.moveInput.x;
        moveDir.Normalize();
        if (controller.isGrounded && velocity < 0.0f)
        {
            velocity = -1.0f;
        }
        else
        {
            velocity += playerData.gravity * playerData.gravityMultiplier * Time.deltaTime;
        }
        moveDir.y = velocity;

        if (moveDir != Vector3.zero)
        {
            Vector3 move = moveDir * playerData.moveSpeed * Time.deltaTime;
            controller.Move(move);
        }
    }

    void RotationHandle()
    {
        GameManager.Instance.GetMouseData(out Vector3 hitPoint, out Vector3 dirToMouse);
        LookAt(dirToMouse);
    }

    public void SwitchSlot(PlayerSelectSlot slot)
    {
        this.slot = slot;
        mouseVisual.SwitchMouseVisual(slot);
        switch (slot)
        {
            case PlayerSelectSlot.Melee:
                ResetRangeDelay();
                break;
            case PlayerSelectSlot.Gun:
                break;
            case PlayerSelectSlot.Fence:
                ResetRangeDelay();
                break;
            case PlayerSelectSlot.Cannon:
                ResetRangeDelay();
                break;
            case PlayerSelectSlot.Barrel:
                ResetRangeDelay();
                break;
            case PlayerSelectSlot.Thorn:
                ResetRangeDelay();
                break;
            case PlayerSelectSlot.Flag:
                ResetRangeDelay();
                break;
        }
    }

    public bool IsSlot(PlayerSelectSlot slot)
    {
        return this.slot == slot;
    }

    void UpdateSlot()
    {
        switch (slot)
        {
            case PlayerSelectSlot.Melee:
                break;
            case PlayerSelectSlot.Gun:
                DecreaseRangeAttackDelay();
                break;
            case PlayerSelectSlot.Fence:
                break;
            case PlayerSelectSlot.Cannon:
                break;
            case PlayerSelectSlot.Barrel:
                break;
            case PlayerSelectSlot.Thorn:
                break;
            case PlayerSelectSlot.Flag:
                break;
        }
    }

    void DecreaseAttackDelay()
    {
        if (curAttackDelay > 0)
        {
            curAttackDelay -= Time.deltaTime;
            if (curAttackDelay <= 0)
            {
                curAttackDelay = 0;
            }
        }
    }

    public bool CanMeleeAttack()
    {
        return curAttackDelay <= 0 && IsSlot(PlayerSelectSlot.Melee);
    }

    public IEnumerator MeleeAttack()
    {
        meleeHitbox.SetActive(true);
        curAttackDelay = playerData.meleeAttackDelay;
        yield return new WaitForSeconds(playerData.meleeAttackDuration);
        meleeHitbox.SetActive(false);
    }

    public void LookAt(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public bool CanRangeAttack()
    {
        return curRangeDelay <= 0 && IsSlot(PlayerSelectSlot.Gun);
    }

    public void RangeAttack()
    {
        GameObject bulletObj = Instantiate(playerData.rangeAttackPrefab.gameObject, rangeAttackSpawnPosition.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Setup(Faction.Player, transform.forward, playerData.rangeAttackBulletSpeed, playerData.rangeAttackBulletTime);
        curRangeDelay = playerData.rangeAttackDelay;
    }

    void DecreaseRangeAttackDelay()
    {
        if (curRangeDelay > 0)
        {
            curRangeDelay -= Time.deltaTime;
            if (curRangeDelay <= 0)
            {
                curRangeDelay = 0;
            }
        }
    }

    void ResetRangeDelay()
    {
        if (curRangeDelay > 0)
        {
            curRangeDelay = playerData.rangeAttackDelay;
        }
    }

    public bool CanPressObj()
    {
        if (mouseVisual != null &&
            IsSlot(PlayerSelectSlot.Cannon) ||
            IsSlot(PlayerSelectSlot.Fence) ||
            IsSlot(PlayerSelectSlot.Thorn) ||
            IsSlot(PlayerSelectSlot.Barrel) ||
            IsSlot(PlayerSelectSlot.Flag))
        {
            return mouseVisual.CanPressObject();
        }

        return false;
    }

}
