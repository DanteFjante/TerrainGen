using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Worldgen;

[Serializable]
public struct Map
{

    public float3[] vectors;
    public Color32[] colors;

    [ReadOnly]
    public int width;
    
    [ReadOnly]
    public int height;

    public float scale;

    private JobHandle _handle;
    public Map(int width, int height, float scale)
    {
        this.width = width;
        this.height = height;
        
        vectors = new float3[width * height];
        colors = new Color32[width * height];

        this.scale = scale;
        _handle = default;
        ResetMap();
    }
    
    public void ResetMap()
    {
        MapCreateJob jobs = new MapCreateJob();
        jobs.scale = scale;
        jobs.width = width;
        jobs.returnArray = new NativeArray<float3>(vectors, Allocator.TempJob);

        while (!_handle.IsCompleted)
        {
            Debug.Log("Map handle is busy resetting map");
        }
        
        _handle = jobs.Schedule(height * width, 100);
        _handle.Complete();
        vectors = jobs.returnArray.ToArray();
        jobs.returnArray.CopyTo(vectors);
        jobs.returnArray.Dispose();
    }
    
    public void Colorize(Color lowColor, Color highcolor, float fromHeight, float toHeight)
    {
        MapColorizeJob jobs = new MapColorizeJob();
        jobs.colors = new NativeArray<Color32>(colors, Allocator.TempJob);
        jobs.vectors = new NativeArray<float3>(vectors, Allocator.TempJob);
        jobs.lowColor = lowColor;
        jobs.highColor = highcolor;
        jobs.fromHeight = fromHeight;
        jobs.toHeight = toHeight;

        while (!_handle.IsCompleted)
        {
            Debug.Log("Map handle is busy colorizing map");
        }
        
        _handle = jobs.Schedule(height * width, 100);
        _handle.Complete();
        colors = jobs.colors.ToArray();
        jobs.colors.Dispose();
        jobs.vectors.Dispose();
    }

    public bool IsReady()
    {
        return _handle.IsCompleted;
    }

    public void SetColor(int x, int y, Color color)
    {
        colors[x + y * width] = color;
    }
    
    public void SetHeight(int x, int y, float height)
    {
        float3 pos = vectors[x + y * width];
        pos.y = height;
        vectors[x + y * width] = pos;
    }
    
    public void SetPosition(int x, int y, float3 position)
    {
        vectors[x + y * width] = position;
    }

    public float GetHeight(int x, int y)
    {
        return vectors[x + y * width].y;
    }
    
    public Vector3 GetPosition(int x, int y)
    {
        return vectors[x + y * width];
    }
    
    public Color GetColor(int x, int y)
    {
        return colors[x + y * width];
    }
    
    public struct MapPoint
    {
        public Vector3 position;
        public Color color;
        public Biome biome;
    }
}

