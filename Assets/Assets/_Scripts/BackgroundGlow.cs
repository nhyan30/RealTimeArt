using UnityEngine;

public class PatternMaterialOverride : MonoBehaviour
{
    public Color tint = new Color(0.5f, 1f, 0.9f, 0.85f);

    void Start()
    {
        // Override all child line materials with unique tinted instance
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderers)
        {
            Material mat = new Material(r.sharedMaterial);
            mat.color = tint;
            r.material = mat;
        }
    }
}