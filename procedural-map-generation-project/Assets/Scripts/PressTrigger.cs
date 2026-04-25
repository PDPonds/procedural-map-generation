using UnityEngine;

public class PressTrigger : MonoBehaviour
{
    [SerializeField] Vector3 colliderSize;
    [SerializeField] Vector3 colliderOffset;

    MeshRenderer meshRen;

    int colliderCount = 0;

    public void Setup()
    {
        meshRen = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        Collider[] cols = Physics.OverlapBox(transform.position + colliderOffset, colliderSize / 2);
        colliderCount = cols.Length;
        if (cols.Length > 0){ meshRen.material = GameManager.Instance.gameData.noPress; }
        else { meshRen.material = GameManager.Instance.gameData.canPress; }
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
