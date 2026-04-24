using UnityEngine;

[ExecuteInEditMode]
public class BoardBorder : MonoBehaviour
{
    [Header("Border Settings")]
    public float boardSize = 6f;
    public float borderWidth = 0.04f;
    public Color glowColor = new Color(0.7f, 0.9f, 1f, 1f);
    public float outerGlowSize = 0.15f;

    private GameObject borderObj;
    private GameObject glowObj;

    void Start() => BuildBorder();

    void BuildBorder()
    {
        if (borderObj != null) DestroyImmediate(borderObj);
        if (glowObj != null) DestroyImmediate(glowObj);

        borderObj = CreateBorderQuad("Border", boardSize, borderWidth, glowColor, 0);
        borderObj.transform.SetParent(transform, false);

        Color glowCol = glowColor; glowCol.a = 0.25f;
        glowObj = CreateBorderQuad("BorderGlow",
            boardSize + outerGlowSize * 2f,
            borderWidth + outerGlowSize * 2f,
            glowCol, -0.01f);
        glowObj.transform.SetParent(transform, false);
    }

    GameObject CreateBorderQuad(string name, float size, float width, Color color, float zOffset)
    {
        GameObject go = new GameObject(name);
        go.transform.localPosition = new Vector3(0, 0, zOffset);
        CreateEdge(go, "Top", size, width, color, 0, size / 2f);
        CreateEdge(go, "Bottom", size, width, color, 0, -size / 2f);
        CreateEdge(go, "Left", width, size, color, -size / 2f, 0);
        CreateEdge(go, "Right", width, size, color, size / 2f, 0);
        return go;
    }

    void CreateEdge(GameObject parent, string name, float w, float h, Color color, float x, float y)
    {
        GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Quad);
        DestroyImmediate(edge.GetComponent<Collider>());
        edge.name = name;
        edge.transform.SetParent(parent.transform, false);
        edge.transform.localPosition = new Vector3(x, y, 0);
        edge.transform.localScale = new Vector3(w, h, 1);
        var mat = new Material(Shader.Find("UI/Default"));
        mat.color = color;
        edge.GetComponent<MeshRenderer>().material = mat;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) BuildBorder();
#endif
    }
}