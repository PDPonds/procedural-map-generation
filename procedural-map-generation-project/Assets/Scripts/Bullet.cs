using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 dir;
    float speed;
    Faction faction = Faction.None;

    public void Setup(Faction faction, Vector3 dir, float speed, float duration)
    {
        this.dir = dir;
        this.speed = speed;
        this.faction = faction;
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (faction)
        {
            case Faction.Player:
                Destroy(gameObject);
                break;
            case Faction.Enemy:
                Destroy(gameObject);
                break;
        }
    }

}
