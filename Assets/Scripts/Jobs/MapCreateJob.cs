using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Jobs
{
    [BurstCompile]
    public struct MapCreateJob : IJobParallelFor
    {
        public int width;
        public float scale;
        public NativeArray<float3> returnArray;


        public void Execute(int index)
        {
            int x = index % width;
            int y = index / width;
            returnArray[index] = new float3(x * scale, 0 , y * scale);
        }
    }
}