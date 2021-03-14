using System;
using GenerationTools;

namespace Worldgen
{
    [Serializable]
    public struct NoiseGenerator
    {
        public NoiseData data;
        public float amplitude;
        public float heightOffset;
        public bool warpPosition;
        public GeneratorCombineType type;
        public bool enabled;
        
        public enum GeneratorCombineType
        {
            Mean,
            Add,
            Subtract,
            Multiply
        }
    }
}