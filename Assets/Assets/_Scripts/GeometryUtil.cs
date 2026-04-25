using UnityEngine;

public static class GeometryUtil
{
    public static bool PointInsideTriangle(
        Vector3 p,
        Triangle t)
    {
        Vector3 v0 = t.c - t.a;
        Vector3 v1 = t.b - t.a;
        Vector3 v2 = p - t.a;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom =
            1f / (dot00 * dot11 - dot01 * dot01);

        float u =
            (dot11 * dot02 - dot01 * dot12) * invDenom;

        float v =
            (dot00 * dot12 - dot01 * dot02) * invDenom;

        return
            u >= 0 &&
            v >= 0 &&
            (u + v < 1);
    }
}