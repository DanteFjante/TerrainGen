using UnityEngine;

namespace Worldgen
{
    public class World : MonoBehaviour
    {
        private Map[] maps;

        public Transform generateFollow;

        private void Awake()
        {
            if(maps == null)
                maps = new Map[256 * 256];
        }
    }
}