using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Worldgen;

namespace Jobs
{
    [BurstCompile]
    public struct NoiseGenJob : IJobParallelFor
    {
        public NoiseGenerator noise;
        public NativeArray<float3> returnVectors;

        public void Execute(int index)
        {
            float3 returnVector = returnVectors[index];
            float y = returnVector.y;
            
            returnVector.y = noise.data.GetNoiseValue(returnVector) * noise.amplitude + noise.heightOffset;
            
            if(noise.warpPosition)
                noise.data.WarpDomain(ref returnVector);
            returnVectors[index] = returnVector;


            returnVector.y = noise.type switch
            {
                NoiseGenerator.GeneratorCombineType.Add => y + (returnVector.y - y),
                NoiseGenerator.GeneratorCombineType.Subtract => y - (returnVector.y - y),
                NoiseGenerator.GeneratorCombineType.Multiply => y * (returnVector.y - y),
                NoiseGenerator.GeneratorCombineType.Mean => y + (returnVector.y - y != 0 ? (returnVector.y - y) / 2 : 0),
                _ => returnVector.y
            };
        }
    }
}