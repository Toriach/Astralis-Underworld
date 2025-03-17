using System.Collections;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts.Map.The_Grid
{
    public static class BlocksSideChecker
    {
        public static void CheckSides(GridCell[,] CellsTable)
        {
            for (int x = 0; x < CellsTable.GetLength(0); x++)
            {
                for (int z = 0; z < CellsTable.GetLength(1); z++)
                {
                    for (int y = 0; y < CellsTable[x, z].blocks.Length; y++)
                    {
                        if (CellsTable[x, z].blocks[y].IsDestroyed) continue;

                        // left
                        if (x != 0)
                        {
                            if (CellsTable[x - 1, z].blocks[y].IsDestroyed)
                            {
                                CellsTable[x, z].blocks[y].HaveLeft = true;
                            }
                        }
                        else
                        {
                            CellsTable[x, z].blocks[y].HaveLeft = true;
                        }

                        // right
                        if (x != CellsTable.GetLength(0) - 1)
                        {
                            if (CellsTable[x + 1, z].blocks[y].IsDestroyed)
                            {
                                CellsTable[x, z].blocks[y].HaveRight = true;
                            }
                        }
                        else
                        {
                            CellsTable[x, z].blocks[y].HaveRight = true;
                        }

                        // down
                        if (z != 0)
                        {
                            if (CellsTable[x, z - 1].blocks[y].IsDestroyed)
                            {
                                CellsTable[x, z].blocks[y].HaveDown = true;
                            }
                        }
                        else
                        {
                            CellsTable[x, z].blocks[y].HaveDown = true;
                        }

                        // up
                        if (z != CellsTable.GetLength(1) - 1)
                        {
                            if (CellsTable[x, z + 1].blocks[y].IsDestroyed)
                            {
                                CellsTable[x, z].blocks[y].HaveUp = true;
                            }
                        }
                        else
                        {
                            CellsTable[x, z].blocks[y].HaveUp = true;
                        }

                        // top
                        if (y != CellsTable[x, z].blocks.Length -1)
                        {
                           if( CellsTable[x, z].blocks[y + 1].IsDestroyed)
                            {
                                CellsTable[x, z].blocks[y].HaveTop = true;
                            }
                        }
                        else
                        {
                            CellsTable[x, z].blocks[y].HaveTop = true;
                        }
                    }
                }
            }
        }

    }
}