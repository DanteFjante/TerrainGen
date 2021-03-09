using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Worldgen
{
    [BurstCompile]
    public struct NoiseGenJob : IJobParallelFor
    {
        public NoiseGenerator noise;
        public NativeArray<float3> returnVectors;


        public void Execute(int index)
        {
            var returnVector = returnVectors[index];
            returnVector.y = noise.data.GetNoiseValue(returnVector) * noise.amplitude + noise.heightOffset;
            if(noise.warpPosition)
                noise.data.WarpDomain(ref returnVector);
            returnVectors[index] = returnVector;
        }
    }
}