using System;
using System.Collections.Generic;
using System.Linq;
using GenerationTools;
using Unity.Mathematics;
using UnityEngine;


[Serializable]
public class GeneratorBundle
{

    public List<GeneratorBundleData> Generators;

    public Vector3 GetPosAt(float x, float z)
    {

        float combinedWeight = CombinedWeight;
        
        Vector3 ret = new Vector3(x, 0, z);

        foreach (var data in Generators)
        {
            float mult = combinedWeight / data.weight;
            float y;
            
            if(data.warpPosition)
            {
                float3 warp = new float3(x, 0, z);
                data.generator.WarpDomain(ref warp);
                ret.x = Mathf.Lerp(ret.x, warp.x, mult);
                ret.z = Mathf.Lerp(ret.z, warp.z, mult);
            }
            y = data.generator.GetNoiseValue(x, z);
            y *= data.amplify;
            y += data.heightOffset;
            switch (data.type)
            {
                case GeneratorCombineType.Add:
                    y = y * mult + ret.y;
                    break;
                case GeneratorCombineType.Multiply:
                    y = y * mult * ret.y;
                    break;
                case GeneratorCombineType.Subtract:
                    y = y * mult - ret.y;
                    break;
                case GeneratorCombineType.Mean:
                    y = (y * mult + ret.y + 0.000001f) / 2;
                    break;
            }
            ret.y = y;
        }

        return ret;
    }
    
    public float GetHeightAt(float x, float z)
    {

        float combinedWeight = CombinedWeight;

        float rety = 0;

        foreach (var data in Generators)
        {
            float mult =   1 / (combinedWeight / data.weight);
            float y;
            
            y = data.generator.GetNoiseValue(x, z);
            y *= data.amplify;
            y += data.heightOffset;
            switch (data.type)
            {
                case GeneratorCombineType.Add:
                    y = y * mult + rety;
                    break;
                case GeneratorCombineType.Multiply:
                    y = y * mult * rety;
                    break;
                case GeneratorCombineType.Subtract:
                    y = y * mult - rety;
                    break;
                case GeneratorCombineType.Mean:
                    y = (y * mult + rety + 0.000001f) / 2;
                    break;
            }
            rety = y;
        }

        return rety;
    }


    public float CombinedWeight => Generators.Select(p => p.weight).Sum();

    [Serializable]
    public class GeneratorBundleData
    {
        public NoiseData generator;
        public GeneratorCombineType type;
        public float weight;
        public float amplify;
        public float heightOffset;
        public bool warpPosition;
    }

    public enum GeneratorCombineType
    {
        Mean,
        Add,
        Subtract,
        Multiply
    }
}
