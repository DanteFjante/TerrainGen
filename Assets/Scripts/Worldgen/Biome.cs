using GenerationTools;
using UnityEngine;

namespace Worldgen
{

    [CreateAssetMenu]
    public class Biome : ScriptableObject
    {
        public GeneratorBundle Generator;
    }
}