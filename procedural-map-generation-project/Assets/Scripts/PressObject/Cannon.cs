using UnityEngine;

public class Cannon : IDamageable
{
    public override int curHP { get; set; }
    public override int maxHP { get; set; }
    public override Faction faction { get; set; }

    private void Awake()
    {
        faction = Faction.Player;
        curHP = startHP;
        maxHP = startHP;
    }


    public override void Death()
    {
        Destroy(gameObject);
    }
}
