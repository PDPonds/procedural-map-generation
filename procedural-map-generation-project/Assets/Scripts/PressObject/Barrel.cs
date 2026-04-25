using UnityEngine;

public class Barrel : IDamageable
{
    public override int curHP { get; set; }
    public override int maxHP { get; set; }

    private void Awake()
    {
        curHP = startHP;
        maxHP = startHP;
    }

    public override void Death()
    {
        //Create Explosion
        Destroy(gameObject);
    }
}
