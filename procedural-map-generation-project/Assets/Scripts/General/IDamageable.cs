using UnityEngine;

public abstract class IDamageable : MonoBehaviour
{
    public int startHP;

    public abstract Faction faction { get; set; }

    public abstract int curHP { get; set; }

    public abstract int maxHP { get; set; }

    public abstract void Death();

    public void TakeDamage(int damage)
    {
        curHP -= damage;
        if (curHP <= 0)
        {
            Death();
        }
    }


}
