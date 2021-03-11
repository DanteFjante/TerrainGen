using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Jobs
{
    [BurstCompile]
    public struct MapColorizeJob : IJobParallelFor
    {
        public Color lowColor;
        public Color highColor;

        public float fromHeight, toHeight;
        
        public NativeArray<float3> vectors;
        public NativeArray<Color32> colors;
        
        
        public void Execute(int index)
        {
            float y = vectors[index].y;
            if (y >= fromHeight && y < toHeight)
                colors[index] = Color.LerpUnclamped(
                    lowColor,
                    highColor,
                    Mathf.InverseLerp(fromHeight, toHeight, y));
        }
    }
}