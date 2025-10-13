using Assets.Astralis_Underworld.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map
{
    [System.Serializable]
    public class BlockFace 
    {
        public FaceDirection Direction;
        public List<Vector3> Vertices = new List<Vector3>();
        public List<int> Triangles = new List<int>();
        public List<Vector2> UVs = new List<Vector2>();

        public void SetUVs(Rect uvRect)
        {
            int tesselationScale = GameConstants.tessellationScale;
            UVs.Clear();

            for (int i = 0; i <= tesselationScale; i++)
            {
                for (int j = 0; j <= tesselationScale; j++)
                {
                    // j → poziom (U), i → pion (V)
                    float u = Mathf.Lerp(uvRect.xMin, uvRect.xMax, (float)j / tesselationScale);
                    float v = Mathf.Lerp(uvRect.yMin, uvRect.yMax, (float)i / tesselationScale);
                    UVs.Add(new Vector2(u, v));
                }
            }
        }

    }
}