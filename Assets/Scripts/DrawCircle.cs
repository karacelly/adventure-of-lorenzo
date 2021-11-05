using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public int segments = 40;
    public int xradius = 10, yradius = 10;

    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        initRadius();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initRadius()
    {
        lineRenderer.SetVertexCount(segments + 1);
        lineRenderer.useWorldSpace = false;
        CreatePoints();
        lineRenderer.gameObject.SetActive(false);
    }

    void CreatePoints()
    {
        float x;
        float y = 0f;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            lineRenderer.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
        }
    }

    
}
