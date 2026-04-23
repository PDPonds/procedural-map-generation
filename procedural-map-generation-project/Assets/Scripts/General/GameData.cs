using UnityEngine;

[CreateAssetMenu(menuName = "GameData")]
public class GameData :ScriptableObject
{
    public Transform player_Prefab;
    public Transform cameraHolder_Prefab;
    public Transform mouseVisual_Prefab;
    public LayerMask mouseLayerMark;
}
