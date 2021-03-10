using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Worldgen;


[Serializable]
public class GeneratorBundle
{

    public List<NoiseGenerator> Generators;

    public float3[] GenerateMapNoise(float3[] vectors)
    {
        List<float3[]> maps = new List<float3[]>();
        foreach (var gen in Generators)
        {
            NoiseGenJob jobs = new NoiseGenJob();
            jobs.noise = gen;
            jobs.returnVectors = new NativeArray<float3>(vectors, Allocator.TempJob);
                
            JobHandle handle = jobs.Schedule(vectors.Length, 64);
            handle.Complete();
            maps.Add(jobs.returnVectors.ToArray());
            jobs.returnVectors.Dispose();
        }
    }
    
    public float GetHeightAt(float x, float z)
    {

        float rety = 0;

        foreach (var data in Generators)
        {
            float y;
            
            y = data.data.GetNoiseValue(x, z);
            y *= data.amplitude;
            y += data.heightOffset;
            switch (data.type)
            {
                case NoiseGenerator.GeneratorCombineType.Add:
                    y = rety + y - rety;
                    break;
                case NoiseGenerator.GeneratorCombineType.Multiply:
                    y = rety * (y - rety);
                    break;
                case NoiseGenerator.GeneratorCombineType.Subtract:
                    y = rety - (y - rety);
                    break;
                case NoiseGenerator.GeneratorCombineType.Mean:
                    if (y == 0 && rety == 0)
                        y = 0;
                    else
                        y = rety + (y - rety) / 2;
                    break;
            }
            rety = y;
        }

        return rety;
    }


}
