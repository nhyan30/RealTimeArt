using UnityEngine;

[ExecuteInEditMode]
public class BackgroundGlow : MonoBehaviour
{
    public Color bgColor = new Color(0.06f, 0.08f, 0.14f, 1f);
    public Color vignetteColor = new Color(0.02f, 0.04f, 0.08f, 1f);

    void Start()
    {
        var cam = GetComponent<Camera>();
        if (cam != null)
            cam.backgroundColor = bgColor;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Simple pass-through (for future post-processing)
        Graphics.Blit(src, dest);
    }
}