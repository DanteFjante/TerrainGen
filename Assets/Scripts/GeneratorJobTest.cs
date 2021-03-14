using System;
using System.Collections.Generic;
using UnityEngine;

namespace Worldgen
{
    public class GeneratorJobTest : MonoBehaviour
    {

        public Biome biome;

        public int width, height;
        public float scale;

        public Material mapMaterial;
        public List<Map> maps;

        private void Start()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject go = new GameObject("Heightmap", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(Map));
                    go.transform.position = new Vector3(x * 255 * scale, 0, y * 255 * scale);
                    go.transform.localScale = Vector3.one * scale;
                    go.transform.parent = transform;
                    go.GetComponent<MeshRenderer>().material = mapMaterial;
                    maps.Add(go.GetComponent<Map>());
                }
            }

            foreach (var map in maps)
            {
                LoadMesh(map);
            }
        }

        public void LoadMesh(Map map)
        {
            print("Generate start " + DateTime.Now.Ticks / 100000);
            map.ResetMap();
            
            map.vectors = biome.Generator.GenerateMapNoise(map.vectors);

            map.Colorize(Color.yellow, Color.white, 5, 25);
            map.Colorize(Color.red, Color.yellow, 0, 5);
            map.Colorize(Color.blue, Color.cyan, -10, 0);
            
            map.UpdateMesh();
            print("Generate end " + DateTime.Now.Ticks / 100000);
        }

        private void OnValidate()
        {
            if(Application.isPlaying)
                foreach (var map in maps)
                {
                    LoadMesh(map);
                }
        }
    }
}