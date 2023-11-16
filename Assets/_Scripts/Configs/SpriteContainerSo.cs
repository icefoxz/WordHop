using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteContainer", menuName = "资源/图形容器")]
public class SpriteContainerSo : ScriptableObject
{
    [SerializeField]private Sprite[] data;

    public Sprite[] Data => data;

    // 可以在编辑器中手动调用这个方法来清除HideFlags
#if UNITY_EDITOR
    [Button]
#endif
    public void ClearHideFlags()
    {
        foreach (var sprite in data)
        {
            if (sprite != null)
            {
                // 这里我们清除所有可能阻碍保存sprite的标志
                sprite.hideFlags &= ~HideFlags.DontSave;
                sprite.hideFlags &= ~HideFlags.HideAndDontSave;
            }
        }

        // 强制保存更改
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}