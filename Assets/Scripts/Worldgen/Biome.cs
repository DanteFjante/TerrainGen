using UnityEngine;
using UnityEngine.Events;

namespace Worldgen
{

    [CreateAssetMenu]
    public class Biome : ScriptableObject
    {
        
        [HideInInspector]
        public UnityAction validate;
        
        public GeneratorBundle Generator;
        
        public float GetHeight(float x, float z)
        {
            return Generator.GetHeightAt(x, z);
        }
        
        public Vector3 GetPosAt(float x, float z)
        {
            return Generator.GetPosAt(x, z);
        }

        private void OnValidate()
        {
            Generator.Generators.ForEach(p => p.generator.Apply());
            validate?.Invoke();
        }
    }
}