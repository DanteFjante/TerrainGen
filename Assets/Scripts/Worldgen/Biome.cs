using UnityEngine;
using UnityEngine.Events;

namespace Worldgen
{

    [CreateAssetMenu]
    public class Biome : ScriptableObject
    {
        public GeneratorBundle Generator;
    }
}