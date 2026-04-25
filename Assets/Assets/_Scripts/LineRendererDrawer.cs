using System.Collections.Generic;
using UnityEngine;

public static class LineRendererDrawer
{
    static List<GameObject> lines =
        new List<GameObject>();


    static int insetLayers = 8;
    static float layerLift = .015f;


    public static void Draw(
        List<Triangle> tris,
        Material mat)
    {
        Clear();

        HashSet<string> edgeCache =
            new HashSet<string>();

        foreach (var t in tris)
        {
            DrawInsetTriangle(
                t,
                mat,
                edgeCache
            );
        }
    }


    static void DrawInsetTriangle(
        Triangle tri,
        Material mat,
        HashSet<string> edgeCache)
    {
        Vector3 center = tri.Center;

        for (int i = 0; i < insetLayers; i++)
        {
            float t = i / (float)insetLayers;

            Vector3 lift =
                Vector3.up *
                (i * layerLift);

            Vector3 A =
                Vector3.Lerp(
                    tri.a,
                    center,
                    t
                ) + lift;

            Vector3 B =
                Vector3.Lerp(
                    tri.b,
                    center,
                    t
                ) + lift;

            Vector3 C =
                Vector3.Lerp(
                    tri.c,
                    center,
                    t
                ) + lift;

            DrawUniqueLine(A, B, mat, edgeCache);
            DrawUniqueLine(B, C, mat, edgeCache);
            DrawUniqueLine(C, A, mat, edgeCache);
        }
    }


    static void DrawUniqueLine(
        Vector3 a,
        Vector3 b,
        Material mat,
        HashSet<string> cache)
    {
        string key = EdgeKey(a, b);

        if (cache.Contains(key))
            return;

        cache.Add(key);

        GameObject go =
            new GameObject("Line");

        LineRenderer lr =
            go.AddComponent<LineRenderer>();

        lr.material = mat;

        lr.startWidth = .03f;
        lr.endWidth = .03f;

        lr.positionCount = 2;

        lr.SetPosition(0, a);
        lr.SetPosition(1, b);

        lr.numCapVertices = 8;

        lines.Add(go);
    }


    static string EdgeKey(
        Vector3 a,
        Vector3 b)
    {
        string p1 =
            a.ToString("F2");

        string p2 =
            b.ToString("F2");

        return
            string.Compare(p1, p2) < 0
                ? p1 + p2
                : p2 + p1;
    }


    static void Clear()
    {
        foreach (var l in lines)
        {
            if (l != null)
                GameObject.Destroy(l);
        }

        lines.Clear();
    }
}