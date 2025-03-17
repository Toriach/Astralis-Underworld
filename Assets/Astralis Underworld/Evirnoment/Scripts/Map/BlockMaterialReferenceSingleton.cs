using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map
{
    public class BlockMaterialReferenceSingleton : MonoSingleton<BlockMaterialReferenceSingleton>
    {
        public Mesh mesh;
        public List<Material> materials;
    }
}