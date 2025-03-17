using Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid;
using System.Collections.Generic;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map
{
    public static class GridCellsOperations 
    {
        public static List<GridCell> SortByDistance(List<GridCell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].IsEmpty) continue;
                cells[i].GetDistanceToPlayer();
            }

            cells.Sort((one, secound) =>
                one.DistanceToPlayer.CompareTo(secound.DistanceToPlayer));

            return cells;
        }
    }
}