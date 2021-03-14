using System.Linq;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Map : MonoBehaviour
{

    public MapSize size = MapSize.Large;
    
    public float3[] vectors;
    public Color32[] colors;

    public float scale = 1;

    private MeshFilter _filter;

    void Awake()
    {
        ResetMap();
    }
    
    public MeshFilter GetMeshFilter()
    {
        return _filter ??= GetComponent<MeshFilter>();
    }
    

    public void ResetMap()
    {
        vectors = new float3[(int) size * (int) size];
        colors = new Color32[(int) size * (int) size];

        JobHandle handle = new JobHandle();
        MapCreateJob jobs = new MapCreateJob();
        jobs.scale = scale;
        jobs.width = (int) size;
        jobs.returnArray = new NativeArray<float3>(vectors, Allocator.Persistent);

        while (!handle.IsCompleted) {}
        handle = jobs.Schedule((int)size * (int)size, 100);
        handle.Complete();
        vectors = jobs.returnArray.ToArray();
        jobs.returnArray.Dispose();
    }
    
    public void Colorize(Color lowColor, Color highcolor, float fromHeight, float toHeight)
    {
        JobHandle handle = new JobHandle();
        MapColorizeJob jobs = new MapColorizeJob();
        jobs.colors = new NativeArray<Color32>(colors, Allocator.Persistent);
        jobs.vectors = new NativeArray<float3>(vectors, Allocator.Persistent);
        jobs.lowColor = lowColor;
        jobs.highColor = highcolor;
        jobs.fromHeight = fromHeight;
        jobs.toHeight = toHeight;

        while (!handle.IsCompleted) {}
        handle = jobs.Schedule((int) size * (int) size, 100);
        handle.Complete();
        colors = jobs.colors.ToArray();
        jobs.colors.Dispose();
        jobs.vectors.Dispose();
    }
    public void UpdateMesh()
    {
        
        Mesh mesh = new Mesh();
        
        TrianglesAndUvsForPlaneJob job = new TrianglesAndUvsForPlaneJob((int) size);

        JobHandle handle = job.Schedule();
        
        handle.Complete();

        while (!handle.IsCompleted) {}

        mesh.vertices = vectors.Select(p => new Vector3(p.x, p.y, p.z)).ToArray();
        mesh.colors32 = colors;

        mesh.triangles = job.returnIndicies.ToArray();
        mesh.uv = job.returnUvs.Select(p => new Vector2(p.x, p.y)).ToArray();

        job.returnUvs.Dispose();
        job.returnIndicies.Dispose();

        GetMeshFilter().mesh = mesh;

    }
    
    public enum MapSize
    {
        Large = 256,
        Medium = 127,
        Small = 64
    }
}

