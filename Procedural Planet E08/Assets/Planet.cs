using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public int meshByCubeFace = 1;
    int numberOfMeshes;
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColourGenerator colourGenerator = new ColourGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;


    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        numberOfMeshes = 6 * meshByCubeFace * meshByCubeFace;

        if (meshFilters == null || meshFilters.Length == 0)
        //if (meshFilters == null || meshFilters.Length == 0 || meshFilters.Length != numberOfMeshes)
        {
            meshFilters = new MeshFilter[numberOfMeshes];
        }
        terrainFaces = new TerrainFace[numberOfMeshes];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };



        float divider = 1/(float)meshByCubeFace;
        float axisAStart;
        float axisBStart;
   
        axisAStart = - (1-divider);
        axisBStart = - (1-divider);
        //Debug.Log("diviser " + divider + " axisAStart " + axisAStart);
        float[] axisAOffsets = new float[meshByCubeFace];
        float[] axisBOffsets = new float[meshByCubeFace];

        //Debug.Log("axisAOffsets ");
        for (int a = 0; a < meshByCubeFace; a++){
            axisAOffsets[a] = axisAStart + a * 2 * divider;
            axisBOffsets[a] = axisBStart + a * 2 * divider;

            //Debug.Log(axisAOffsets[a]);
        }

        

        int i = 0;
        for (int direction_i = 0; direction_i < 6; direction_i++)
        {
            int face_i = 0;
            
            foreach (float axisAOffset in axisAOffsets){

                foreach (float axisBOffset in axisBOffsets){


                    if (meshFilters[i] == null)
                    {
                        GameObject meshObj = new GameObject("mesh_"+direction_i+"_"+face_i);
                        meshObj.transform.parent = transform;

                        meshObj.AddComponent<MeshRenderer>();
                        meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                        meshFilters[i].sharedMesh = new Mesh();
                    }
                    meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

                    terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[direction_i], divider, axisAOffset, axisBOffset);
                    bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == direction_i;
                    meshFilters[i].gameObject.SetActive(renderFace);
                    //Debug.Log("iteration direction " + direction_i + " face "+ face_i);

                    i++;
                    face_i++;
                }

            }

           

        }


        /*for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }*/
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {
        for (int i = 0; i < numberOfMeshes; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColours()
    {
        colourGenerator.UpdateColours();
        for (int i = 0; i < numberOfMeshes; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }
    }
}
