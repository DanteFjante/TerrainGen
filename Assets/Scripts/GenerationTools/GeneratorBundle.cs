using System;
using System.Collections.Generic;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Worldgen;

namespace GenerationTools
{
    [Serializable]
    public class GeneratorBundle
    {

        public List<NoiseGenerator> Generators;
        
        public void UpdateGenerators()
        {
            foreach (var gen in Generators)
            {
                gen.data.Apply();
            }
        }

        public float3[] GenerateMapNoise(float3[] vectors)
        {
            foreach (var gen in Generators)
            {
                NoiseGenJob jobs = new NoiseGenJob();
                jobs.noise = gen;
                jobs.returnVectors = new NativeArray<float3>(vectors, Allocator.TempJob);
                
                JobHandle handle = jobs.Schedule(vectors.Length, 64);
                handle.Complete();
                while (!handle.IsCompleted) {}
                vectors = jobs.returnVectors.ToArray();
                jobs.returnVectors.Dispose();
            }

            return vectors;
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
}
