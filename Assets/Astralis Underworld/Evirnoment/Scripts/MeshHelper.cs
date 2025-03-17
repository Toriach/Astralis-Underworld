using Assets.Astralis_Underworld.Scripts;
using System.Collections.Generic;
using UnityEngine;

public static class MeshHelper
{
    public static List<List<int>> GroupVerticesByPosition(List<Vector3> vertices, float epsilon = 0.001f)
    {
        // Utwórz słownik, gdzie kluczem będzie zaokrąglona pozycja wierzchołka, a wartością będzie lista indeksów wierzchołków o tej samej pozycji
        Dictionary<Vector3, List<int>> groupedVertices = new Dictionary<Vector3, List<int>>();

        // Iteruj przez wszystkie wierzchołki
        for (int i = 0; i < vertices.Count; i++)
        {
            // Zaokrągl pozycję wierzchołka, aby uniknąć problemów z precyzją zmiennoprzecinkową
            Vector3 roundedPosition = RoundToEpsilon(vertices[i], epsilon);

            // Sprawdź, czy zaokrąglona pozycja wierzchołka jest już w słowniku
            if (groupedVertices.ContainsKey(roundedPosition))
            {
                // Jeśli tak, dodaj indeks wierzchołka do listy indeksów dla danej pozycji
                groupedVertices[roundedPosition].Add(i);
            }
            else
            {
                // Jeśli nie, utwórz nową listę indeksów i dodaj ją do słownika
                List<int> indices = new List<int>();
                indices.Add(i);
                groupedVertices.Add(roundedPosition, indices);
            }
        }

        // Zamień słownik na listę list indeksów
        List<List<int>> result = new List<List<int>>(groupedVertices.Values);

        return result;
    }

    private static Vector3 RoundToEpsilon(Vector3 vector, float epsilon)
    {
        return new Vector3(RoundToEpsilon(vector.x, epsilon), RoundToEpsilon(vector.y, epsilon), RoundToEpsilon(vector.z, epsilon));
    }

    private static float RoundToEpsilon(float value, float epsilon)
    {
        return Mathf.Round(value / epsilon) * epsilon;
    }

}
