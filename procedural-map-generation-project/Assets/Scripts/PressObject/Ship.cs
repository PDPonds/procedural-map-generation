using UnityEngine;
using UnityEngine.AI;

public class Ship : IDamageable
{
    public override Faction faction { get; set; }
    public override int curHP { get; set; }
    public override int maxHP { get; set; }

    private void Awake()
    {
        faction = Faction.Ship;
        curHP = startHP;
        maxHP = startHP;
    }

    public override void TakeDamageEffect()
    {

    }

    public override void Death()
    {
        Destroy(gameObject);
    }

}
