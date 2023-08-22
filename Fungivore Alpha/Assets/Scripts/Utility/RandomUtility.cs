using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomUtility
{
    private static int globalSeed = 0;

    public static void SetGlobalSeed(int seed)
    {
        globalSeed = seed;
        Debug.Log("global seed set in RandomUtility: " + globalSeed);
    }

    public static void ResetGlobalSeed()
    {
        globalSeed = 0;
        Debug.Log("reset RandomUtility globalSeed to 0");
    }


    // Generate a random integer between min (inclusive) and max (exclusive) based on position hash and global seed
    public static int Range(Vector3 position, int min, int max)
    {
        // Combine entity's position hash with the global seed
        int combinedSeed = Mathf.RoundToInt(position.GetHashCode() + globalSeed) % 100000;

        // Use the combined seed to create a random instance
        System.Random random = new System.Random(combinedSeed);

        // Generate a random integer between min (inclusive) and max (exclusive)
        return random.Next(min, max);
    }

    public static System.Random NewRandom(Vector3 position)
    {
        // Combine entity's position hash with the global seed
        int combinedSeed = Mathf.RoundToInt(position.GetHashCode() + globalSeed) % 100000;

        // Use the combined seed to create a random instance
        System.Random random = new System.Random(combinedSeed);

        return random;
    }

}
