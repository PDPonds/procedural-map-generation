using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            if (damageable.faction != Faction.Ship)
            {
                damageable.TakeDamage(GameManager.Instance.player.curDamage);
            }
        }
    }
}
