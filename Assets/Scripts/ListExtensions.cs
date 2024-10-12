using System;
using System.Collections.Generic;

public static class ListExtensions
{
    private static Random rng = new Random(); // Random number generator

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Generates a list of paired IDs based on the given total number of items and shuffles the list.
    /// </summary>
    /// <param name="list">The list to populate with generated IDs.</param>
    /// <param name="totalItems">The total number of items to generate IDs for (must be an even number).</param>
    public static List<int> GenerateAndShuffleIds(this List<int> list, int totalItems)
    {
        if (totalItems % 2 != 0)
        {
            throw new ArgumentException("Total items must be an even number to ensure pairs.");
        }

        int pairs = totalItems / 2;

        for (int i = 0; i < pairs; i++)
        {
            list.Add(i); // pair 1
            list.Add(i); // pair 2
        }

        list.Shuffle();
        return list;
    }
}

