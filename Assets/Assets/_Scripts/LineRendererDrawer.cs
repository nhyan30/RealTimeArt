using System.Collections.Generic;
using UnityEngine;

public static class LineRendererDrawer
{
    static List<GameObject> lines = new List<GameObject>();

    public static void Draw(List<Triangle> tris, Material mat)
    {
        // Clear old lines
        foreach (var l in lines)
            GameObject.Destroy(l);

        lines.Clear();

        foreach (var t in tris)
        {
            DrawLine(t.a, t.b, mat);
            DrawLine(t.b, t.c, mat);
            DrawLine(t.c, t.a, mat);
        }
    }

    static void DrawLine(Vector3 a, Vector3 b, Material mat)
    {
        GameObject go = new GameObject("Line");
        LineRenderer lr = go.AddComponent<LineRenderer>();

        lr.material = mat;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        lr.positionCount = 2;
        lr.SetPosition(0, a + Vector3.up * 0.02f);
        lr.SetPosition(1, b + Vector3.up * 0.02f);

        lr.numCapVertices = 5;

        lines.Add(go);
    }
}