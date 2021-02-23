using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitRenderer : MonoBehaviour
{
    public LineRenderer lr;

    [Range(3, 36)]
    public int segments;

    private void Awake()
    {
        CalculateEllipse();
    }

    void CalculateEllipse()
    {
        Vector3[] points = new Vector3[segments+1];
        for (int i = 0; i < segments; i++)
        {
            Vector2 pos2D = GameParams.Main.orbit.Evaluate((float)i/segments);
            points[i] = new Vector3(pos2D.x, pos2D.y, 0);
        }
        points[segments] = points[0];
        lr.positionCount = segments + 1;
        lr.SetPositions(points);
    }

    private void OnValidate()
    {
        if(lr != null)
        CalculateEllipse();
    }
}
