using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TessellationPattern : MonoBehaviour
{
    [Header("Visual")]
    public float triangleSize = 0.35f;
    public Color lineColor = new Color(0.55f, 0.92f, 0.95f, 0.9f);
    public float lineWidth = 0.014f;
    public Color glowColor = new Color(0.3f, 0.7f, 0.95f, 0.15f);
    public float glowWidth = 0.05f;

    [Header("Evolution")]
    public int maxStages = 5;
    public float baseExtent = 1.5f;       // radius at stage 1
    public float extentPerStage = 0.75f;   // extra radius per stage

    [Header("Animation")]
    public float animDuration = 0.6f;

    // ── internal state ──
    private int currentStage;
    private HashSet<string> existingKeys = new HashSet<string>();
    private List<GameObject> allLines = new List<GameObject>();
    private List<GameObject> allGlows = new List<GameObject>();
    private List<GameObject> newLines = new List<GameObject>();
    private List<GameObject> newGlows = new List<GameObject>();
    private Material lineMat;
    private Material glowMat;
    private float animTime;
    private bool isAnimating;

    public bool CanEvolve => currentStage < maxStages;
    public int Stage => currentStage;

    // ────────────────────────────────────────────
    void Awake()
    {
        lineMat = new Material(Shader.Find("UI/Default")) { color = lineColor };
        glowMat = new Material(Shader.Find("UI/Default")) { color = glowColor };
    }

    /// <summary>Call from BoardClickHandler to advance one stage.</summary>
    public bool Evolve()
    {
        if (!CanEvolve) return false;

        // finish any running animation instantly before starting the next
        if (isAnimating) FinishAnimation();

        currentStage++;
        float extent = baseExtent + (currentStage - 1) * extentPerStage;
        GenerateRing(extent);

        animTime = 0f;
        isAnimating = true;
        return true;
    }

    // ── ring generation (only NEW edges are added) ──
    void GenerateRing(float extent)
    {
        float h = triangleSize * Mathf.Sqrt(3f) / 2f;
        int gridRange = Mathf.CeilToInt(extent / triangleSize) + 3;

        var candidates = new List<(Vector2 a, Vector2 b, float d)>();

        for (int row = -gridRange; row <= gridRange; row++)
        {
            bool odd = ((row % 2) + 2) % 2 == 1;
            for (int col = -gridRange; col <= gridRange; col++)
            {
                float cx = col * triangleSize + (odd ? triangleSize * 0.5f : 0f);
                float cy = row * h;
                float d = new Vector2(cx, cy).magnitude;
                if (d > extent) continue;

                // upward triangle
                Vector2 a1 = new Vector2(cx - triangleSize * 0.5f, cy - h / 3f);
                Vector2 b1 = new Vector2(cx + triangleSize * 0.5f, cy - h / 3f);
                Vector2 c1 = new Vector2(cx, cy + h * 2f / 3f);
                TryAdd(a1, b1, d, candidates);
                TryAdd(b1, c1, d, candidates);
                TryAdd(c1, a1, d, candidates);

                // downward triangle
                Vector2 a2 = new Vector2(cx - triangleSize * 0.5f, cy + h * 2f / 3f);
                Vector2 b2 = new Vector2(cx + triangleSize * 0.5f, cy + h * 2f / 3f);
                Vector2 c2 = new Vector2(cx, cy - h / 3f);
                TryAdd(a2, b2, d, candidates);
                TryAdd(b2, c2, d, candidates);
                TryAdd(c2, a2, d, candidates);
            }
        }

        // reveal from centre outward
        var sorted = candidates.OrderBy(x => x.d).ToList();

        foreach (var edge in sorted)
        {
            var glow = MakeLine(edge.a, edge.b, glowWidth, glowMat, 0.001f);
            var line = MakeLine(edge.a, edge.b, lineWidth, lineMat, 0f);
            SetAlpha(glow, 0f);
            SetAlpha(line, 0f);

            allGlows.Add(glow); newGlows.Add(glow);
            allLines.Add(line); newLines.Add(line);
        }
    }

    // ── helpers ──
    void TryAdd(Vector2 a, Vector2 b, float d, List<(Vector2, Vector2, float)> list)
    {
        string k = Key(a, b);
        if (existingKeys.Contains(k)) return;
        existingKeys.Add(k);
        list.Add((a, b, d));
    }

    string Key(Vector2 a, Vector2 b)
    {
        bool swap = a.x > b.x || (a.x == b.x && a.y > b.y);
        Vector2 lo = swap ? b : a;
        Vector2 hi = swap ? a : b;
        return $"{lo.x:F5},{lo.y:F5}_{hi.x:F5},{hi.y:F5}";
    }

    GameObject MakeLine(Vector2 s, Vector2 e, float w, Material mat, float z)
    {
        var go = new GameObject("L");
        go.transform.SetParent(transform, false);

        Vector2 dir = (e - s).normalized;
        float len = Vector2.Distance(s, e);
        Vector2 mid = (s + e) * 0.5f;

        go.transform.localPosition = new Vector3(mid.x, mid.y, z);
        go.transform.localRotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        var mesh = new Mesh();
        float hw = w * 0.5f;
        mesh.vertices = new[]
        {
            new Vector3(-len / 2, -hw, 0), new Vector3( len / 2, -hw, 0),
            new Vector3( len / 2,  hw, 0), new Vector3(-len / 2,  hw, 0)
        };
        mesh.triangles = new[] { 0, 2, 1, 0, 3, 2 };
        mesh.RecalculateBounds();

        go.AddComponent<MeshFilter>().mesh = mesh;
        var r = go.AddComponent<MeshRenderer>();
        r.material = new Material(mat);          // instance for per-line alpha
        return go;
    }

    void SetAlpha(GameObject go, float a)
    {
        var c = go.GetComponent<MeshRenderer>().material.color;
        c.a = a;
        go.GetComponent<MeshRenderer>().material.color = c;
    }

    void FinishAnimation()
    {
        foreach (var l in newLines) l.GetComponent<MeshRenderer>().material.color = lineColor;
        foreach (var g in newGlows) g.GetComponent<MeshRenderer>().material.color = glowColor;
        newLines.Clear();
        newGlows.Clear();
        isAnimating = false;
    }

    // ── per-frame reveal animation ──
    void Update()
    {
        if (!isAnimating) return;

        animTime += Time.deltaTime;
        float t = Mathf.Clamp01(animTime / animDuration);
        float eased = 1f - (1f - t) * (1f - t) * (1f - t);   // ease-out cubic

        int n = newLines.Count;
        if (n == 0) { isAnimating = false; return; }

        float cursor = eased * (n + 5f);   // +5 so the last few lines still fade in

        for (int i = 0; i < n; i++)
        {
            float a = Mathf.Clamp01(cursor - i);
            a = 1f - (1f - a) * (1f - a);  // per-line ease

            Color lc = lineColor; lc.a = a * lineColor.a;
            newLines[i].GetComponent<MeshRenderer>().material.color = lc;

            Color gc = glowColor; gc.a = a * glowColor.a;
            newGlows[i].GetComponent<MeshRenderer>().material.color = gc;
        }

        if (t >= 1f) FinishAnimation();
    }

    // ── reset / cleanup ──
    public void Clear()
    {
        FinishAnimation();
        currentStage = 0;
        existingKeys.Clear();

        foreach (var l in allLines)
            if (l) { Destroy(l.GetComponent<MeshRenderer>().material); Destroy(l); }
        foreach (var g in allGlows)
            if (g) { Destroy(g.GetComponent<MeshRenderer>().material); Destroy(g); }

        allLines.Clear();
        allGlows.Clear();
    }

    void OnDestroy()
    {
        if (lineMat) Destroy(lineMat);
        if (glowMat) Destroy(glowMat);
    }
}