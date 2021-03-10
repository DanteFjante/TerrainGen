using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Worldgen
{
    public struct TrianglesAndUvsForPlaneJob : IJob
    {
        public int width;
        public int height;

        public NativeArray<int> returnIndicies;
        public NativeArray<float2> returnUvs;
        
        public void Execute()
        {
            returnIndicies = new NativeArray<int>((width-1) * (height - 1) * 6, Allocator.TempJob);
            returnUvs = new NativeArray<float2>(width * height, Allocator.TempJob);

            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    returnIndicies[i + j * width] = i + j * width;
                    returnIndicies[i + j * width + 1] = i + j * width + width;
                    returnIndicies[i + j * width + 2] = i + j * width + 1;

                    returnIndicies[i + j * width + 3] = i + j * width + width;
                    returnIndicies[i + j * width + 4] = i + j * width + width + 1;
                    returnIndicies[i + j * width + 5] = i + j * width + 1;
                    
                    returnUvs[i + j * width] = new float2(i, j);
                }
            }
            
        }
    }
}