using System;
using System.Collections.Generic;
using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.Terrain
{
    public static class DataProcessing
    {
        public static List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 1),   //North
            new Vector2Int(1, 1),   //North-East
            new Vector2Int(1, 0),   //East
            new Vector2Int(1, -1),  //South-East
            new Vector2Int(0, -1),  //South
            new Vector2Int(-1, -1), //South-West
            new Vector2Int(-1, 0),  //West
            new Vector2Int(-1, 1)   //North-West
        };

        public static List<Vector2Int> FindLocalMaxima(float[,] dataMatrix, int xCoord, int zCoord)
        {
            List<Vector2Int> maximas = new List<Vector2Int>();
            for (int x = 0; x < dataMatrix.GetLength(0); ++x)
            {
                for (int y = 0; y < dataMatrix.GetLength(1); ++y)
                {
                    float noiseVal = dataMatrix[x, y];

                    if (CheckNeighbours(dataMatrix, x, y, (neighbourNoise) => neighbourNoise < noiseVal))
                        maximas.Add(new Vector2Int(xCoord + x, zCoord + y));
                }
            }

            return maximas;
        }

        private static bool CheckNeighbours(float[,] dataMatrix, int x, int y, Func<float, bool> successCondition)
        {
            foreach (var dir in directions)
            {
                var newPos = new Vector2Int(x + dir.x, y + dir.y);

                if (newPos.x < 0 || newPos.x >= dataMatrix.GetLength(0) || newPos.y < 0 || newPos.y >= dataMatrix.GetLength(1))
                    continue;

                if (successCondition(dataMatrix[x + dir.x, y + dir.y]) == false)
                    return false;
            }

            return true;
        }
    }
}