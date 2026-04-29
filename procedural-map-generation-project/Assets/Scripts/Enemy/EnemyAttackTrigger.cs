using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            if (damageable.faction == Faction.Player)
            {
                Enemy enemy = transform.parent.GetComponent<Enemy>();
                damageable.TakeDamage(enemy.curDamage);
            }
        }
    }
}
