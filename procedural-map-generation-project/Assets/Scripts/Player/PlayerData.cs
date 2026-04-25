using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("----- Controller -----")]
    [Header("- Move")]
    public float moveSpeed = 10f;
    [Header("- Rotation")]
    public float rotationSpeed = 10f;
    [Header("- Gravity")]
    public float gravity = -9.81f;
    public float gravityMultiplier = 3f;
    [Header("----- Melee Attack -----")]
    public float meleeAttackDuration = 0.25f;
    public float meleeAttackDelay = 0.5f;
    [Header("----- Range Attack -----")]
    public Transform rangeAttackPrefab;
    public float rangeAttackDelay = 1f;
    public float rangeAttackBulletSpeed = 50f;
    public float rangeAttackBulletTime = 1f;

}
