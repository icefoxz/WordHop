using UnityEngine;

[CreateAssetMenu(fileName = "SpriteContainer", menuName = "资源/图形容器")]
public class SpriteContainerSo : ScriptableObject
{
    [SerializeField]private Sprite[] data;

    public Sprite[] Data => data;
}