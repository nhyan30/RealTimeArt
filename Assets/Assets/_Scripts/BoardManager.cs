using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public Material lineMaterial;

    [Header("Board")]
    public float boardSize = 5f;

    private List<Triangle> triangles = new List<Triangle>();


    void Start()
    {
        InitializeBoard();
        LineRendererDrawer.Draw(triangles, lineMaterial);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                AddPoint(hit.point);
            }
        }
    }


    void InitializeBoard()
    {
        Vector3[] corners =
        {
            transform.TransformPoint(
                new Vector3(-boardSize,0,-boardSize)),

            transform.TransformPoint(
                new Vector3(boardSize,0,-boardSize)),

            transform.TransformPoint(
                new Vector3(boardSize,0,boardSize)),

            transform.TransformPoint(
                new Vector3(-boardSize,0,boardSize))
        };

        Vector3 center =
            (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;

        triangles.Clear();

        triangles.Add(new Triangle(corners[0], corners[1], center));
        triangles.Add(new Triangle(corners[1], corners[2], center));
        triangles.Add(new Triangle(corners[2], corners[3], center));
        triangles.Add(new Triangle(corners[3], corners[0], center));
    }


    void AddPoint(Vector3 p)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            if (GeometryUtil.PointInsideTriangle(p, triangles[i]))
            {
                Triangle t = triangles[i];

                triangles.RemoveAt(i);

                triangles.Add(
                    new Triangle(t.a, t.b, p));

                triangles.Add(
                    new Triangle(t.b, t.c, p));

                triangles.Add(
                    new Triangle(t.c, t.a, p));

                break;
            }
        }

        LineRendererDrawer.Draw(
            triangles,
            lineMaterial
        );
    }
}