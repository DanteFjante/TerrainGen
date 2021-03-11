using System.Linq;
using Jobs;
using Unity.Jobs;
using UnityEngine;
using Worldgen;

public static class MeshMaker
{
    public static Mesh CreateFloor(Map map)
    {
        
        Mesh mesh = new Mesh();

        TrianglesAndUvsForPlaneJob job = new TrianglesAndUvsForPlaneJob();
        job.height = (int) map.size;
        job.width = (int) map.size;

        JobHandle handle = job.Schedule();
        
        handle.Complete();

        while (!handle.IsCompleted) ;

        mesh.triangles = job.returnIndicies.ToArray();
        mesh.uv = job.returnUvs.Select(p => new Vector2(p.x, p.y)).ToArray();

        job.returnUvs.Dispose();
        job.returnIndicies.Dispose();
        
        mesh.vertices = map.vectors.Select(p => new Vector3(p.x, p.y, p.z)).ToArray();
        mesh.colors32 = map.colors;

        return mesh;
    }

}
