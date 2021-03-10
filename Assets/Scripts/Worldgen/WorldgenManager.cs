using System;
using UnityEngine;
using Worldgen;

public class WorldgenManager : MonoBehaviour
{

    public MeshFilter meshFilter;
    

    public int width, height;
    public float scale;
    public Biome defaultBiome;
    public Map map;

    private void Awake()
    {

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

        for (int i = 0; i < width+1; i++)
        {
            for (int j = 0; j < height+1; j++)
            {
                Vector3 pos = new Vector3(i,defaultBiome.Generator.GetHeightAt(i * scale, j * scale), j);

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
