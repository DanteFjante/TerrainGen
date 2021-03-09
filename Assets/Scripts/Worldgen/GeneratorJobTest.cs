using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Worldgen
{
    public class GeneratorJobTest : MonoBehaviour
    {

        public NoiseGenerator TestNoise;

        public int width, height;
        public float scale;
        
        public MeshFilter filter;

        private void Start()
        {
            LoadMesh();
        }

        public void LoadMesh()
        {
            TestNoise.data.Apply();
            
            Map map = new Map(width, height, scale);

            NoiseGenJob jobs = new NoiseGenJob();
            while (!map.IsReady()) ;
            jobs.noise = TestNoise;
            jobs.returnVectors = new NativeArray<float3>(map.vectors, Allocator.TempJob);
                
            JobHandle handle = jobs.Schedule(width * height, 64);
            handle.Complete();

            while (!handle.IsCompleted)
            {
            }

            map.vectors = jobs.returnVectors.ToArray();
            jobs.returnVectors.Dispose();

            map.Colorize(Color.red, Color.yellow, 0, TestNoise.amplitude);
            map.Colorize(Color.blue, Color.cyan, -10, 0);

            while (!map.IsReady()) ;
            filter.mesh = MeshMaker.CreateFloor(map);
        }

        private void OnValidate()
        {
            if(Application.isPlaying)
                LoadMesh();
        }
    }
}