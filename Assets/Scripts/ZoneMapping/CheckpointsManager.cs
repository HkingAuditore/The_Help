using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    public List<Checkpoint.Checkpoint> checkpoints = new List<Checkpoint.Checkpoint>();

    //地图构建
    private readonly Dictionary<int, Vector3> _safeCheckpointsPositions = new Dictionary<int, Vector3>();
    private List<Vector3Int> _triangles = new List<Vector3Int>();

    public ComputeShader mappingShader;
    public RenderTexture map;

    private readonly int _renderTexSize = 256;
    private int _mapSize = 1024;

    private struct TriangleData
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
    }


    private void Start()
    {
        checkpoints.Sort((a,b) => b.index.CompareTo(a.index));

        GeneratePositions();

        _triangles.Add(new Vector3(0, 3, 2));
        _triangles.Add(new Vector3(0, 1, 2));
        _triangles.Add(new Vector3(2, 1, 4));

        map = new RenderTexture(_renderTexSize, _renderTexSize, 24);

        RenderZoneMap();
    }

    private void GeneratePositions()
    {
        Dictionary<int, Vector3> tmpDict = new Dictionary<int, Vector3>();
        foreach (Checkpoint.Checkpoint checkpoint in (from checkpoint1 in checkpoints 
                                                        where checkpoint1.isSafe
                                                            select checkpoint1))
        {
            if (!_safeCheckpointsPositions.ContainsKey(checkpoint.index))
            {
                tmpDict.Add(checkpoint.index,checkpoint.GetPosition_2D());
            }
        }
    }



    public void GenerateTriangles()
    {
        List<Vector3Int> tris = new List<Vector3Int>();
        foreach (var checkpoint in checkpoints)
        {
            foreach (Checkpoint.Checkpoint linkedCheckpoint in checkpoint.linkedCheckpoints)
            {
                foreach (Checkpoint.Checkpoint linkedLinkedCheckpoint in linkedCheckpoint.linkedCheckpoints)
                {
                    if (linkedLinkedCheckpoint.linkedCheckpoints.Contains(checkpoint))
                    {
                        List<int> tmpTri = (from i in new[] {checkpoint.index, linkedCheckpoint.index, linkedLinkedCheckpoint.index}
                            orderby i
                            select i).ToList();
                        Vector3Int tri = new Vector3Int(tmpTri[0],tmpTri[1],tmpTri[2]);
                        if(!tris.Contains(tri))tris.Add(tri);
                    }
                }

            }
        }

        _triangles = tris;

    }

    public void RenderZoneMap()
    {
        var data = new TriangleData[_triangles.Count];

        for (var i = 0; i < _triangles.Count; i++)
        {
            data[i].a.x = _safeCheckpointsPositions[(int) _triangles[i].x].x;
            data[i].a.y = _safeCheckpointsPositions[(int) _triangles[i].x].z;

            data[i].b.x = _safeCheckpointsPositions[(int) _triangles[i].y].x;
            data[i].b.y = _safeCheckpointsPositions[(int) _triangles[i].y].z;

            data[i].c.x = _safeCheckpointsPositions[(int) _triangles[i].z].x;
            data[i].c.y = _safeCheckpointsPositions[(int) _triangles[i].z].z;
            Debug.Log(data[i].a);
            Debug.Log(data[i].b);
            Debug.Log(data[i].c);
        }


        var buffer = new ComputeBuffer(data.Length, 6 * 4);
        buffer.SetData(data);

        var kernelHandle = mappingShader.FindKernel("CSMain");
        mappingShader.SetFloat("triangleCount", _triangles.Count);
        Debug.Log("COUNT：" + _triangles.Count);
        mappingShader.SetBuffer(kernelHandle, "dataBuffer", buffer);

        map.enableRandomWrite = true;
        map.Create();

        mappingShader.SetTexture(kernelHandle, "Result", map);
        mappingShader.Dispatch(kernelHandle, 256 / 8, 256 / 8, 1);
    }
}