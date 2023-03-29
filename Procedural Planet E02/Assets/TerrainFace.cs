using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {

    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    float divider_;
    float axisAOffset_;
    float axisBOffset_;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp, float divider, float axisAOffset, float axisBOffset)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        divider_ = divider;
        axisAOffset_ = axisAOffset;
        axisBOffset_ = axisBOffset;
        Debug.Log("divider_ " + divider_ + " axisAOffset_ "+ axisAOffset_ + " axisBOffset_ "+ axisBOffset_);

    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        Debug.Log("ConstructMesh " + localUp);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Debug.Log("x " + x + " y "+ y);
                int i = x + y * resolution;
                Debug.Log("i " +i);
                Vector2 percent = new Vector2(x, y)   / (resolution - 1);
                Debug.Log("percent " +percent);
                Debug.Log("divider_ " + divider_ + " axisAOffset_ "+ axisAOffset_ + " axisBOffset_ "+ axisBOffset_);
                Vector3 pointOnUnitCube = localUp + ((percent.x - .5f) * 2 * divider_ + axisAOffset_) * axisA + ((percent.y - .5f) * 2 * divider_ + axisBOffset_) * axisB;
                Debug.Log("pointOnUnitCube " +pointOnUnitCube);
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
