using System;
using System.Threading;

public static class ThreadSafeRandom
{
    private static readonly Random _globalSeed = new Random();
    private static readonly ThreadLocal<Random> _localInstance = new ThreadLocal<Random>(NewRandom);

    // Generates a new Random instance for each thread with a unique seed
    private static Random NewRandom()
    {
        lock (_globalSeed)
        {
            return new Random(_globalSeed.Next());
        }
    }

    // Public methods to generate random numbers
    public static int Next() => _localInstance.Value.Next();
    public static int Next(int maxValue) => _localInstance.Value.Next(maxValue);
    public static int Next(int minValue, int maxValue) => _localInstance.Value.Next(minValue, maxValue);
    public static double NextDouble() => _localInstance.Value.NextDouble();
    public static void NextBytes(byte[] buffer) => _localInstance.Value.NextBytes(buffer);
}