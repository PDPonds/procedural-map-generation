using UnityEngine;

public class Cannon : IDamageable
{
    public override int curHP { get; set; }
    public override int maxHP { get; set; }

    public override void Death()
    {
        Destroy(gameObject);
    }
}
