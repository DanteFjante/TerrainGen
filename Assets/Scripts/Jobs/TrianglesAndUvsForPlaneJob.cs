using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Jobs
{
    [BurstCompile]
    public struct TrianglesAndUvsForPlaneJob : IJob
    {
        public int width;
        public int height;

        public NativeArray<int> returnIndicies;
        public NativeArray<float2> returnUvs;


        public TrianglesAndUvsForPlaneJob(int size)
        {
            width = size;
            height = size;
            returnIndicies = new NativeArray<int>((width-1) * (height - 1) * 6, Allocator.TempJob);
            returnUvs = new NativeArray<float2>(width * height, Allocator.TempJob);
        }
        
        public void Execute()
        {
            int index = 0;
            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    int pos = i + j * width;
                    returnIndicies[index + 0] = pos;
                    returnIndicies[index + 1] = pos + width;
                    returnIndicies[index + 2] = pos + 1;
                    
                    returnIndicies[index + 3] = pos + width;
                    returnIndicies[index + 4] = pos + width + 1;
                    returnIndicies[index + 5] = pos + 1;
                    index += 6;
                    
                    returnUvs[i + j * width] = new float2(i, j);
                }
            }
            
        }
    }
}