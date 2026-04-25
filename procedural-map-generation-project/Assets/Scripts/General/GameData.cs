using UnityEngine;

[CreateAssetMenu(menuName = "GameData")]
public class GameData :ScriptableObject
{
    public Transform ship_Prefab;
    public Transform player_Prefab;
    public Transform cameraHolder_Prefab;
    public Transform mouseVisual_Prefab;
    public LayerMask mouseLayerMark;

    [Header("----- Press Visual -----")]
    public Material canPress;
    public Material noPress;
    [Header("----- Press Obj -----")]
    public Transform fencePrefab;
    public Transform cannonPrefab;
    public Transform barrelPrefab;
    public Transform thornPrefab;
    public Transform flagPrefab;
}
