using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneMapping
{
    public class SafeZoneMap : MonoBehaviour
    {
        private List<Checkpoint.Checkpoint> _checkpoints = new List<Checkpoint.Checkpoint>();
        private Dictionary<int,Vector3> _safeCheckpointsPositions = new Dictionary<int, Vector3>();
        private List<Vector3> _triangles = new List<Vector3>();
        
        public ComputeShader mappingShader;
        public RenderTexture map;

        private int _renderTexSize = 256;
        private int _mapSize = 1024;
        
        struct TriangleData
        {
            public Vector2 a;
            public Vector2 b;
            public Vector2 c;
        }
        

        private void Start()
        {
            _safeCheckpointsPositions.Add(0,new Vector3(0,0,0));
            _safeCheckpointsPositions.Add(1,new Vector3(100,0,0));
            _safeCheckpointsPositions.Add(2,new Vector3(100,0,100));
            _safeCheckpointsPositions.Add(3,new Vector3(0,0,100));
            _safeCheckpointsPositions.Add(4,new Vector3(200,0,100));
            
            _triangles.Add(new Vector3(0,3,2));
            _triangles.Add(new Vector3(0,1,2));
            _triangles.Add(new Vector3(2,1,4));

            map = new RenderTexture(_renderTexSize, _renderTexSize, 24);
            
            RenderZoneMap();
        }
        
        public void RenderZoneMap()
        {

            TriangleData[] data = new TriangleData[_triangles.Count];
            
            for (int i = 0; i < _triangles.Count; i++)
            {
                data[i].a.x = _safeCheckpointsPositions[(int)_triangles[i].x].x;
                data[i].a.y = _safeCheckpointsPositions[(int)_triangles[i].x].z;
                
                data[i].b.x = _safeCheckpointsPositions[(int)_triangles[i].y].x;
                data[i].b.y = _safeCheckpointsPositions[(int)_triangles[i].y].z;
                
                data[i].c.x = _safeCheckpointsPositions[(int)_triangles[i].z].x;
                data[i].c.y = _safeCheckpointsPositions[(int)_triangles[i].z].z;
                Debug.Log(data[i].a);
                Debug.Log(data[i].b);
                Debug.Log(data[i].c);
            }

            

            ComputeBuffer buffer = new ComputeBuffer(data.Length, 6 * 4);
            buffer.SetData(data);
            
            int kernelHandle = mappingShader.FindKernel("CSMain");
            mappingShader.SetFloat("triangleCount", _triangles.Count);
            Debug.Log("COUNT：" + _triangles.Count);
            mappingShader.SetBuffer(kernelHandle ,"dataBuffer", buffer);
            
            map.enableRandomWrite = true;
            map.Create();
            
            mappingShader.SetTexture(kernelHandle, "Result", map);
            mappingShader.Dispatch(kernelHandle, 256/8, 256/8, 1);
        }
    }
}
