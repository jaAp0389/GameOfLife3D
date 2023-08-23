using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;

class MeshBuilder : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] GameObject mesh;

    MeshFilter meshFilter;

    List<CombineInstance> blockData;
    List<List<CombineInstance>> blockDataLists;

    private List<Mesh> meshes = new List<Mesh>();

    private void Awake()
    {
        meshFilter = Instantiate(mesh, Vector3.zero,
            Quaternion.identity).GetComponent<MeshFilter>();
        FlushLists();
    }

    public void FlushLists()
    {
        blockData = new List<CombineInstance>();
        blockDataLists = new List<List<CombineInstance>>();
    }

    public void AddBlockData(Vector3 _pos)
    {
        meshFilter.transform.position = _pos;
        CombineInstance newCI = new CombineInstance
        {
            mesh = meshFilter.sharedMesh,
            transform = meshFilter.transform.localToWorldMatrix,
        };
        blockData.Add(newCI);
    }

    public void CreateMeshLists()
    {
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());
        for (int i = 0; i < blockData.Count; i++)
        {
            vertexCount += blockData[i].mesh.vertexCount;
            if (vertexCount > 65536)
            {
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
                continue;
            }
            blockDataLists[blockDataLists.Count-1].Add(blockData[i]);
        }
    }

    public void CreateMesh()
    {
        Transform container = new GameObject("Meshys").transform;
        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject g = new GameObject("Meshy");
            g.transform.parent = container;
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.material = material;
            mf.mesh.CombineMeshes(data.ToArray());
            meshes.Add(mf.mesh);
        }
    }
}

