using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    [Range(2,256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public int meshByCubeFace = 2;
    int numberOfMeshes;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;
     

	void Initialize()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);

        numberOfMeshes = 6 * meshByCubeFace * meshByCubeFace;
        Debug.Log("numberOfMeshes  "+ numberOfMeshes.ToString());


        
        if (meshFilters == null || meshFilters.Length == 0 || meshFilters.Length != numberOfMeshes)
        {
            meshFilters = new MeshFilter[numberOfMeshes];
        }else{
            /*Debug.Log("meshFilters.Length  "+ meshFilters.Length);
            for (int mesh_i = 0; mesh_i < meshFilters.Length; mesh_i++){
                DestroyImmediate(meshFilters[mesh_i], true);
            }*/
            //meshFilters = new MeshFilter[numberOfMeshes];
        }

        

        terrainFaces = new TerrainFace[numberOfMeshes];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };


        float divider = 1/(float)meshByCubeFace;
        float axisAStart;
        float axisBStart;
   
        axisAStart = - (1-divider);
        axisBStart = - (1-divider);
        Debug.Log("diviser " + divider + " axisAStart " + axisAStart);
        float[] axisAOffsets = new float[meshByCubeFace];
        float[] axisBOffsets = new float[meshByCubeFace];

        Debug.Log("axisAOffsets ");
        for (int a = 0; a < meshByCubeFace; a++){
            axisAOffsets[a] = axisAStart + a * 2 * divider;
            axisBOffsets[a] = axisBStart + a * 2 * divider;

            Debug.Log(axisAOffsets[a]);
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

                        meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                        meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                        meshFilters[i].sharedMesh = new Mesh();
                    }

                    terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[direction_i], divider, axisAOffset, axisBOffset);
                    Debug.Log("iteration direction " + direction_i + " face "+ face_i);

                    i++;
                    face_i++;
                }

            }

           

        }

        /*for (int i = 0; i < numberOfMeshes; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
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
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColours()
    {
        foreach (MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colourSettings.planetColour;
        }
    }
}
