using UnityEngine;
using UnityEngine.UI;

public class Test_FogEraser : MonoBehaviour
{
    public Image fogImage; // 雾的Image组件
    public RenderTexture maskTexture; // 擦除遮罩的RenderTexture
    public Shader eraserShader; // 一个简单的擦除Shader
    public Camera uiCamera; // UI相机

    private Material eraserMaterial;

    void Start()
    {
        eraserMaterial = new Material(eraserShader);
        fogImage.material = eraserMaterial;
        fogImage.material.SetTexture("_MaskTex", maskTexture);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fogImage.rectTransform, Input.mousePosition, uiCamera, out pos);
            Vector2 normalizedPoint = Rect.PointToNormalized(fogImage.rectTransform.rect, pos);
            EraseFog(normalizedPoint);
        }
    }

    void EraseFog(Vector2 uv)
    {
        RenderTexture activeTex = RenderTexture.active;
        RenderTexture.active = maskTexture;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskTexture.width, maskTexture.height, 0);

        eraserMaterial.SetPass(0);
        GL.Color(Color.clear);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(uv.x, uv.y);
        GL.Vertex3(uv.x * maskTexture.width - 10, uv.y * maskTexture.height - 10, 0);
        GL.TexCoord2(uv.x, uv.y);
        GL.Vertex3(uv.x * maskTexture.width + 10, uv.y * maskTexture.height - 10, 0);
        GL.TexCoord2(uv.x, uv.y);
        GL.Vertex3(uv.x * maskTexture.width + 10, uv.y * maskTexture.height + 10, 0);
        GL.TexCoord2(uv.x, uv.y);
        GL.Vertex3(uv.x * maskTexture.width - 10, uv.y * maskTexture.height + 10, 0);
        GL.End();

        GL.PopMatrix();

        RenderTexture.active = activeTex;
    }
}