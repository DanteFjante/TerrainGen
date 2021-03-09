using System;
using UnityEngine;
using Worldgen;

public class WorldgenManager : MonoBehaviour
{

    public MeshFilter meshFilter;
    

    public int width, height;
    public float scale;
    public Biome defaultBiome;

    private void Awake()
    {
        defaultBiome.validate += () =>
            {
                if(meshFilter != null)
                {
                Map map = createMap();
                meshFilter.mesh = MeshMaker.CreateFloor(map);
            };
        };
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(meshFilter != null && meshFilter.mesh.vertexCount < width * height)
        {
            Map map = createMap();
            meshFilter.mesh = MeshMaker.CreateFloor(map);
        }
    }

    private Map createMap()
    {
        Map map = new Map(width+1, height+1, scale);

        for (int i = 0; i < width+1; i++)
        {
            for (int j = 0; j < height+1; j++)
            {
                Vector3 pos = defaultBiome.GetPosAt(i * scale, j * scale);

                map.SetPosition(i, j, pos);

                map.SetColor(i, j, 
                    Color.Lerp(
                        new Color(.88f, .6f,.33f),
                        Color.green, 
                        map.GetHeight(i, j) / 5));
            }
        }

        return map;
    }
}
