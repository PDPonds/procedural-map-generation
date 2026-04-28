using UnityEngine;

public class PressTrigger : MonoBehaviour
{
    MeshRenderer meshRen;

    int colliderCount = 0;

    public void Setup()
    {
        meshRen = GetComponent<MeshRenderer>();
        meshRen.sharedMaterial = GameManager.Instance.gameData.canPress;
        colliderCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        colliderCount++;
        meshRen.material = GameManager.Instance.gameData.noPress;
    }

    private void OnTriggerExit(Collider other)
    {
        colliderCount--;
        if (colliderCount <= 0)
        {
            meshRen.material = GameManager.Instance.gameData.canPress;
        }
    }

    public bool NoColliding()
    {
        return colliderCount <= 0;
    }

}
