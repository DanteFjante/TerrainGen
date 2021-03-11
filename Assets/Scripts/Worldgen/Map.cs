using System;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Worldgen;

[RequireComponent(typeof(MeshFilter))]
public class Map : MonoBehaviour
{

    public MapSize size;
    
    public float3[] vectors;
    public Color32[] colors;

    public GeneratorBundle generators;

    public float scale;

    private MeshFilter _filter;
    private JobHandle _handle;

    void Awake()
    {
        

        _handle = default;
        ResetMap();
        
    }

    public void ResetMap()
    {
        vectors = new float3[(int) size * (int) size];
        colors = new Color32[(int) size * (int) size];

        MapCreateJob jobs = new MapCreateJob();
        jobs.scale = scale;
        jobs.width = (int) size;
        jobs.returnArray = new NativeArray<float3>(vectors, Allocator.TempJob);

        while (!_handle.IsCompleted)
        {
            Debug.Log("Map handle is busy resetting map");
        }
        
        _handle = jobs.Schedule((int)size * (int)size, 100);
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
        
        _handle = jobs.Schedule((int) size * (int) size, 100);
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
        colors[x + y * (int) size] = color;
    }
    
    public void SetHeight(int x, int y, float height)
    {
        float3 pos = vectors[x + y * (int) size];
        pos.y = height;
        vectors[x + y * (int) size] = pos;
    }
    
    public void SetPosition(int x, int y, float3 position)
    {
        vectors[x + y * (int) size] = position;
    }

    public float GetHeight(int x, int y)
    {
        return vectors[x + y * (int) size].y;
    }
    
    public Vector3 GetPosition(int x, int y)
    {
        return vectors[x + y * (int) size];
    }
    
    public Color GetColor(int x, int y)
    {
        return colors[x + y * (int) size];
    }
    
    public enum MapSize
    {
        Large = 256,
        Medium = 127,
        Small = 64
    }
}

