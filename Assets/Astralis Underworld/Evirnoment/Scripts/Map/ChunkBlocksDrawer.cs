using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Assets.Astralis_Underworld.Evirnoment.Scripts.Map;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    public class ChunkBlocksDrawer : MonoBehaviour
    {
        private List<Matrix4x4[]> _blockMatrixlist;

        private void Awake()
        {
            enabled = false;
        }

        public void UpdateChunkDrawData(List<Matrix4x4[]> blockMatrixlist)
        {
            _blockMatrixlist = blockMatrixlist;
            enabled = true;
        }
        void Update()
        {
            if (_blockMatrixlist == null) 
            {
                enabled = false; 
                return; 
            }

            for (int i = 0; i < _blockMatrixlist.Count; i++)
            {
                Graphics.DrawMeshInstanced(BlockMaterialReferenceSingleton.instance.mesh, 0, BlockMaterialReferenceSingleton.instance.materials[i], _blockMatrixlist[i]);
            }
        }
    }
}