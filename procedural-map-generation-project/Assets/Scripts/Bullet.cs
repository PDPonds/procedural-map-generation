using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 dir;
    float speed;
    int damage;
    Faction faction = Faction.None;

    public void Setup(Faction faction, Vector3 dir, float speed, float duration, int damage)
    {
        this.dir = dir;
        this.speed = speed;
        this.faction = faction;
        this.damage = damage;
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            switch (faction)
            {
                case Faction.Player:
                    if (faction == Faction.Enemy)
                    {
                        damageable.TakeDamage(damage);
                    }
                    break;
                case Faction.Enemy:
                    if (faction == Faction.Player)
                    {
                        damageable.TakeDamage(damage);
                    }
                    break;
            }

        }

        Destroy(gameObject);

    }

}
