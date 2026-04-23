using UnityEngine;

public class PressTrigger : MonoBehaviour
{
    [SerializeField] Vector3 colliderSize;
    [SerializeField] Vector3 colliderOffset;

    Player player;
    MeshRenderer meshRen;

    int colliderCount = 0;

    public void Setup(Player player)
    {
        this.player = player;
        meshRen = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        Collider[] cols = Physics.OverlapBox(transform.position + colliderOffset, colliderSize / 2);
        colliderCount = cols.Length;
        if (cols.Length > 0)
        {
            meshRen.material = player.playerData.noPress;
        }
        else
        {
            meshRen.material = player.playerData.canPress;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + colliderOffset, colliderSize);
    }

    public bool NoColliding()
    {
        return colliderCount == 0;
    }

}
